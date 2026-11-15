/* ============================================================
   auth.js — warstwa kont i sesji w etapie 1 (localStorage).

   Kontrakt dla pozostałych modułów (patrz docs/02-plan-pracy-i-wdrozenia.md):
     await ForumAuth.register({username, email, password, confirmPassword})
     await ForumAuth.login({email, password, remember})
     ForumAuth.logout()
     ForumAuth.currentUser()        -> {username, email, role, rank, joinedAt, settings, avatar} | null
     ForumAuth.isLoggedIn() / isModerator() / isAdmin()
     await ForumAuth.updateAccount({displayName, email})
     await ForumAuth.changePassword({currentPassword, newPassword, confirmPassword})
     ForumAuth.saveSettings({pageSize, idleMinutes, theme})
     ForumAuth.setAvatar(dataUrl) / clearAvatar() / getAvatar()
     ForumAuth.onAuthChange(listener)
     await ForumAuth.ready           -> seed ukończony (hasła demo zahashowane)

   W etapie 2 moduł zostaje podmieniony na wywołania HTTP do AccountController
   z ASP.NET Identity — API i widoki pozostają bez zmian.
   ============================================================ */
(function () {
    'use strict';

    var USERS_KEY = 'forum.users';
    var SESSION_KEY = 'forum.session';        // sesja "zapamiętaj mnie"
    var SESSION_TAB_KEY = 'forum.session.tab'; // sesja jednorazowa (karta przeglądarki)

    var DEFAULT_SETTINGS = { pageSize: 10, idleMinutes: 30, theme: 'light' };

    /* Konta demonstracyjne — hasła zapisane jako SHA-256 (etykieta: etap 1).
       Odpowiadają danym w SeedData po stronie serwera. */
    var SEED_ACCOUNTS = [
        {
            id: 1, username: 'admin', email: 'admin@forum.pl',
            passwordHash: '3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121',
            role: 'admin', rank: 'Administrator', joinedAt: '2025-09-01T08:00:00Z', avatar: null
        },
        {
            id: 2, username: 'moderator', email: 'mod@forum.pl',
            passwordHash: 'ad6b5fa152af4d281b8b85651ca400a1710a7798e5d899279068d63eaa263cc5',
            role: 'moderator', rank: 'Moderator', joinedAt: '2025-09-20T08:00:00Z', avatar: null
        },
        {
            id: 3, username: 'user', email: 'user@forum.pl',
            passwordHash: 'bc5848f227cc161eb5f68dfe98cb13110a9c843ce69e953a88107d865583d397',
            role: 'user', rank: 'Uczestnik', joinedAt: '2026-05-10T08:00:00Z', avatar: null
        }
    ];

    var listeners = [];
    var lastActivityAt = Date.now();
    var idleTimer = null;

    // --- Narzędzia ---------------------------------------------------------

    function storageGet(key) {
        try { return window.localStorage.getItem(key); } catch (e) { return null; }
    }

    function storageSet(key, value) {
        try { window.localStorage.setItem(key, value); } catch (e) { /* ignore */ }
    }

    function storageRemove(key) {
        try { window.localStorage.removeItem(key); } catch (e) { /* ignore */ }
    }

    function sessionGet(key) {
        try { return window.sessionStorage.getItem(key); } catch (e) { return null; }
    }

    function sessionSet(key, value) {
        try { window.sessionStorage.setItem(key, value); } catch (e) { /* ignore */ }
    }

    function sessionRemove(key) {
        try { window.sessionStorage.removeItem(key); } catch (e) { /* ignore */ }
    }

    function sha256(text) {
        if (!(window.crypto && window.crypto.subtle)) {
            return Promise.reject(new Error('Brak kryptografii w przeglądarce (wymagany HTTPS lub localhost).'));
        }
        var bytes = new TextEncoder().encode(text);
        return window.crypto.subtle.digest('SHA-256', bytes).then(function (buffer) {
            return Array.prototype.map.call(new Uint8Array(buffer), function (b) {
                return b.toString(16).padStart(2, '0');
            }).join('');
        });
    }

    function readJson(raw, fallback) {
        if (!raw) { return fallback; }
        try { return JSON.parse(raw); } catch (e) { return fallback; }
    }

    function readUsers() {
        return readJson(storageGet(USERS_KEY), null);
    }

    function writeUsers(users) {
        storageSet(USERS_KEY, JSON.stringify(users));
    }

    function normalizeUser(raw) {
        if (!raw) { return null; }
        return {
            id: raw.id || 0,
            username: raw.username,
            email: raw.email,
            role: raw.role || 'user',
            rank: raw.rank || 'Nowicjusz',
            joinedAt: raw.joinedAt || new Date().toISOString(),
            passwordHash: raw.passwordHash,
            avatar: raw.avatar || null,
            settings: Object.assign({}, DEFAULT_SETTINGS, raw.settings || {})
        };
    }

    function publicUser(user) {
        return {
            id: user.id,
            username: user.username,
            email: user.email,
            role: user.role,
            rank: user.rank,
            joinedAt: user.joinedAt,
            avatar: user.avatar,
            settings: Object.assign({}, user.settings)
        };
    }

    // --- Sesja -------------------------------------------------------------

    function sessionUsername() {
        return storageGet(SESSION_KEY) || sessionGet(SESSION_TAB_KEY) || null;
    }

    function startSession(username, remember) {
        clearSession();
        if (remember) {
            storageSet(SESSION_KEY, username);
        } else {
            sessionSet(SESSION_TAB_KEY, username);
        }
    }

    function clearSession() {
        storageRemove(SESSION_KEY);
        sessionRemove(SESSION_TAB_KEY);
    }

    function findUser(predicate) {
        var users = readUsers() || [];
        for (var i = 0; i < users.length; i++) {
            var user = normalizeUser(users[i]);
            if (user && predicate(user)) { return user; }
        }
        return null;
    }

    function saveUser(updated) {
        var users = (readUsers() || []).map(function (raw) {
            var user = normalizeUser(raw);
            if (!user) { return raw; }
            var matches = (updated.id && user.id === updated.id)
                || user.username === updated.username;
            return matches ? Object.assign({}, updated) : raw;
        });
        writeUsers(users);
    }

    function notify() {
        var user = currentUser();
        listeners.forEach(function (listener) {
            try { listener(user); } catch (e) { console.error('onAuthChange', e); }
        });
        restartIdleTimer();
    }

    // --- Walidacja (lustrzana wobec ViewModelek) ---------------------------

    function validateRegistration(data) {
        var errors = {};
        var username = (data.username || '').trim();
        var email = (data.email || '').trim();

        if (username.length < 3 || username.length > 30) {
            errors.username = 'Nazwa musi mieć od 3 do 30 znaków.';
        }
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
            errors.email = 'Podaj poprawny adres e-mail.';
        }
        if ((data.password || '').length < 8) {
            errors.password = 'Hasło musi mieć co najmniej 8 znaków.';
        } else if (!(/[A-Za-z]/.test(data.password) && /\d/.test(data.password))) {
            errors.password = 'Hasło musi zawierać co najmniej jedną literę i jedną cyfrę.';
        }
        if (data.password !== data.confirmPassword) {
            errors.confirmPassword = 'Hasła nie są identyczne.';
        }
        return errors;
    }

    // --- API ---------------------------------------------------------------

    function currentUser() {
        var username = sessionUsername();
        if (!username) { return null; }
        var user = findUser(function (u) { return u.username.toLowerCase() === username.toLowerCase(); });
        if (!user) {
            clearSession(); // konto usunięte w tej sesji
            return null;
        }
        return publicUser(user);
    }

    var ready = (function () {
        if (!storageGet(USERS_KEY)) {
            writeUsers(SEED_ACCOUNTS);
        }
        return Promise.resolve();
    })();

    var api = {
        ready: ready,

        currentUser: currentUser,

        isLoggedIn: function () { return currentUser() !== null; },

        isAdmin: function () {
            var user = currentUser();
            return !!user && user.role === 'admin';
        },

        isModerator: function () {
            var user = currentUser();
            return !!user && (user.role === 'admin' || user.role === 'moderator');
        },

        register: function (data) {
            return ready.then(function () {
                var errors = validateRegistration(data);
                var username = (data.username || '').trim();
                var email = (data.email || '').trim();

                if (Object.keys(errors).length === 0) {
                    if (findUser(function (u) { return u.username.toLowerCase() === username.toLowerCase(); })) {
                        errors.username = 'Ta nazwa użytkownika jest już zajęta.';
                    }
                    if (findUser(function (u) { return u.email.toLowerCase() === email.toLowerCase(); })) {
                        errors.email = 'Konto z tym adresem e-mail już istnieje.';
                    }
                }
                if (Object.keys(errors).length > 0) {
                    return { ok: false, errors: errors };
                }

                return sha256(data.password).then(function (hash) {
                    var users = readUsers() || [];
                    var account = {
                        id: users.length + 1,
                        username: username,
                        email: email,
                        passwordHash: hash,
                        role: 'user',
                        rank: 'Nowicjusz',
                        joinedAt: new Date().toISOString(),
                        avatar: null,
                        settings: Object.assign({}, DEFAULT_SETTINGS)
                    };
                    users.push(account);
                    writeUsers(users);
                    startSession(account.username, true);
                    notify();
                    return { ok: true, errors: {} };
                });
            });
        },

        login: function (data) {
            return ready.then(function () {
                var identifier = (data.email || '').trim().toLowerCase();
                var user = findUser(function (u) {
                    return u.email.toLowerCase() === identifier || u.username.toLowerCase() === identifier;
                });

                if (!user) {
                    return { ok: false, errors: { email: 'Nie znaleziono konta o podanym adresie e-mail.' } };
                }

                return sha256(data.password || '').then(function (hash) {
                    if (hash !== user.passwordHash) {
                        return { ok: false, errors: { password: 'Nieprawidłowe hasło.' } };
                    }
                    startSession(user.username, !!data.remember);
                    lastActivityAt = Date.now();
                    notify();
                    return { ok: true, errors: {} };
                });
            });
        },

        logout: function () {
            clearSession();
            notify();
        },

        updateAccount: function (data) {
            var user = currentUser();
            if (!user) { return Promise.resolve({ ok: false, errors: { general: 'Sesja wygasła.' } }); }

            var errors = {};
            var displayName = (data.displayName || '').trim();
            var email = (data.email || '').trim();

            if (displayName.length < 3 || displayName.length > 30) {
                errors.displayName = 'Nazwa musi mieć od 3 do 30 znaków.';
            }
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                errors.email = 'Podaj poprawny adres e-mail.';
            }
            var emailTaken = findUser(function (u) {
                return u.email.toLowerCase() === email.toLowerCase() && u.username !== user.username;
            });
            if (emailTaken) { errors.email = 'Konto z tym adresem e-mail już istnieje.'; }

            if (Object.keys(errors).length > 0) {
                return Promise.resolve({ ok: false, errors: errors });
            }

            var stored = findUser(function (u) { return u.username === user.username; });
            var oldName = stored.username;
            stored.username = displayName;
            stored.email = email;
            saveUser(stored);

            if (oldName !== displayName) {
                clearSession();
                startSession(displayName, true);
            }
            notify();
            return Promise.resolve({ ok: true, errors: {} });
        },

        changePassword: function (data) {
            var user = currentUser();
            if (!user) { return Promise.resolve({ ok: false, errors: { general: 'Sesja wygasła.' } }); }

            var errors = {};
            if ((data.newPassword || '').length < 8) {
                errors.newPassword = 'Nowe hasło musi mieć co najmniej 8 znaków.';
            } else if (!(/[A-Za-z]/.test(data.newPassword) && /\d/.test(data.newPassword))) {
                errors.newPassword = 'Nowe hasło musi zawierać literę i cyfrę.';
            }
            if (data.newPassword !== data.confirmNewPassword) {
                errors.confirmNewPassword = 'Hasła nie są identyczne.';
            }
            if (Object.keys(errors).length > 0) {
                return Promise.resolve({ ok: false, errors: errors });
            }

            var stored = findUser(function (u) { return u.username === user.username; });
            return sha256(data.currentPassword || '').then(function (hash) {
                if (hash !== stored.passwordHash) {
                    return { ok: false, errors: { currentPassword: 'Nieprawidłowe aktualne hasło.' } };
                }
                return sha256(data.newPassword).then(function (next) {
                    stored.passwordHash = next;
                    saveUser(stored);
                    return { ok: true, errors: {} };
                });
            });
        },

        saveSettings: function (partial) {
            var user = currentUser();
            if (!user) { return false; }
            var stored = findUser(function (u) { return u.username === user.username; });
            stored.settings = Object.assign({}, stored.settings, partial);
            saveUser(stored);
            if (partial.theme) {
                window.ForumTheme.set(partial.theme);
            }
            try { // cookie dla paginacji serwerowej (ustawienie liczby wiadomości na stronie)
                document.cookie = 'forum.pageSize=' + encodeURIComponent(stored.settings.pageSize) +
                    '; path=/; max-age=31536000; SameSite=Lax';
            } catch (e) { /* ignore */ }
            notify();
            return true;
        },

        setAvatar: function (dataUrl) {
            var user = currentUser();
            if (!user) { return false; }
            var stored = findUser(function (u) { return u.username === user.username; });
            stored.avatar = dataUrl;
            saveUser(stored);
            notify();
            return true;
        },

        clearAvatar: function () {
            return api.setAvatar(null);
        },

        getAvatar: function () {
            var user = currentUser();
            return user ? user.avatar : null;
        },

        onAuthChange: function (listener) {
            listeners.push(listener);
        }
    };

    // --- Wylogowanie po bezczynności (2 pkt) ------------------------------

    function restartIdleTimer() {
        if (idleTimer) { window.clearInterval(idleTimer); }
        var user = currentUser();
        if (!user) { return; }
        lastActivityAt = Date.now();
        idleTimer = window.setInterval(function () {
            var minutes = (user.settings && user.settings.idleMinutes) || 30;
            if (Date.now() - lastActivityAt >= minutes * 60000) {
                api.logout();
                var path = window.location.pathname;
                if (path.indexOf('/Account/Login') !== 0 && path.indexOf('/Account/Register') !== 0) {
                    window.location.href = '/Account/Login?reason=idle';
                }
            }
        }, 20000);
    }

    ['mousemove', 'keydown', 'click', 'scroll', 'touchstart'].forEach(function (evt) {
        document.addEventListener(evt, function () { lastActivityAt = Date.now(); }, { passive: true });
    });

    window.ForumAuth = api;
    restartIdleTimer();
})();
