/* ============================================================
   account.js — zachowania widoków kont:
     * logowanie / rejestracja: intercept submit -> ForumAuth (walidacja + localStorage)
     * profil: przekierowanie na adres z nazwą użytkownika + uzupełnienie danych sesji
     * ustawienia: wypełnianie formularzy z sesji, zapis (konto, hasło, personalizacja)
     * avatar: walidacja typu, rozmiaru w kB i rozdzielczości przed zapisem
   ============================================================ */
(function () {
    'use strict';

    // --- Pomocnicze --------------------------------------------------------

    function clearErrors(form) {
        form.querySelectorAll('[data-auth-errors]').forEach(function (el) { el.innerHTML = ''; });
        form.querySelectorAll('.field-validation-error').forEach(function (el) {
            el.textContent = '';
            el.classList.add('field-validation-valid');
            el.classList.remove('field-validation-error');
        });
        form.querySelectorAll('.input-validation-error').forEach(function (el) {
            el.classList.remove('input-validation-error');
        });
    }

    function showErrors(form, errors) {
        clearErrors(form);
        var summary = form.querySelector('[data-auth-errors]');
        var messages = Object.keys(errors).map(function (key) { return errors[key]; });

        if (summary && messages.length) {
            var ul = document.createElement('ul');
            messages.forEach(function (text) {
                var li = document.createElement('li');
                li.textContent = text;
                ul.appendChild(li);
            });
            summary.appendChild(ul);
        }

        Object.keys(errors).forEach(function (fieldName) {
            var input = form.querySelector('[name="' + fieldName + '"], #' + fieldName);
            if (input) {
                input.classList.add('input-validation-error');
                var span = input.parentElement
                    ? input.parentElement.parentElement.querySelector('[data-valmsg-for="' + fieldName + '"], [data-valmsg-summary]')
                    : null;
                // asp-validation-for generuje span z data-valmsg-for
                span = form.querySelector('span[data-valmsg-for="' + fieldName + '"]');
                if (span) {
                    span.textContent = errors[fieldName];
                    span.classList.remove('field-validation-valid');
                    span.classList.add('field-validation-error');
                }
            }
        });

        if (summary) {
            summary.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    }

    function showSuccess(form, text) {
        var alert = form.querySelector('[data-form-alert]');
        if (alert) {
            alert.textContent = text;
            alert.classList.remove('d-none');
            window.setTimeout(function () { alert.classList.add('d-none'); }, 4000);
        }
    }

    function loginUrl(returnUrl) {
        return '/Account/Login' + (returnUrl ? '?returnUrl=' + encodeURIComponent(returnUrl) : '');
    }

    // --- Formularz logowania ------------------------------------------------

    function initLoginForm() {
        var form = document.getElementById('loginForm');
        if (!form) { return; }

        var returnUrl = form.getAttribute('data-return-url') || '/';

        form.addEventListener('submit', function (event) {
            event.preventDefault();
            clearErrors(form);

            var email = form.querySelector('[name="Email"]').value.trim();
            var password = form.querySelector('[name="Password"]').value;
            var remember = form.querySelector('[name="RememberMe"]').checked;

            var clientErrors = {};
            if (!email) {
                clientErrors.Email = 'Podaj adres e-mail.';
            } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                clientErrors.Email = 'Podaj poprawny adres e-mail.';
            }
            if (!password) {
                clientErrors.Password = 'Podaj hasło.';
            }
            if (Object.keys(clientErrors).length > 0) {
                showErrors(form, clientErrors);
                return;
            }

            var button = form.querySelector('button[type="submit"]');
            button.disabled = true;

            window.ForumAuth.login({ email: email, password: password, remember: remember })
                .then(function (result) {
                    button.disabled = false;
                    if (result.ok) {
                        window.location.href = returnUrl || '/';
                    } else {
                        showErrors(form, result.errors);
                    }
                })
                .catch(function (error) {
                    button.disabled = false;
                    showErrors(form, { general: error.message });
                });
        });
    }

    // --- Formularz rejestracji ---------------------------------------------

    function initRegisterForm() {
        var form = document.getElementById('registerForm');
        if (!form) { return; }

        form.addEventListener('submit', function (event) {
            event.preventDefault();
            clearErrors(form);

            var data = {
                username: form.querySelector('[name="Username"]').value.trim(),
                email: form.querySelector('[name="Email"]').value.trim(),
                password: form.querySelector('[name="Password"]').value,
                confirmPassword: form.querySelector('[name="ConfirmPassword"]').value
            };
            var accepted = form.querySelector('[name="AcceptTerms"]').checked;

            var clientErrors = {};
            if (data.username.length < 3 || data.username.length > 30) {
                clientErrors.Username = 'Nazwa musi mieć od 3 do 30 znaków.';
            }
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
                clientErrors.Email = 'Podaj poprawny adres e-mail.';
            }
            if (data.password.length < 8) {
                clientErrors.Password = 'Hasło musi mieć co najmniej 8 znaków.';
            } else if (!(/[A-Za-z]/.test(data.password) && /\d/.test(data.password))) {
                clientErrors.Password = 'Hasło musi zawierać co najmniej jedną literę i jedną cyfrę.';
            }
            if (data.password !== data.confirmPassword) {
                clientErrors.ConfirmPassword = 'Hasła nie są identyczne.';
            }
            if (!accepted) {
                clientErrors.AcceptTerms = 'Musisz zaakceptować regulamin, aby się zarejestrować.';
            }
            if (Object.keys(clientErrors).length > 0) {
                showErrors(form, clientErrors);
                return;
            }

            var button = form.querySelector('button[type="submit"]');
            button.disabled = true;

            window.ForumAuth.register(data)
                .then(function (result) {
                    button.disabled = false;
                    if (result.ok) {
                        window.location.href = '/Account/Profile';
                    } else {
                        showErrors(form, result.errors);
                    }
                })
                .catch(function (error) {
                    button.disabled = false;
                    showErrors(form, { general: error.message });
                });
        });
    }

    // --- Profil -------------------------------------------------------------

    function initials(name) {
        if (!name) { return '?'; }
        var parts = String(name).trim().split(/\s+/);
        var first = parts[0] ? parts[0][0] : '?';
        var second = parts.length > 1 ? parts[parts.length - 1][0] : (parts[0] && parts[0].length > 1 ? parts[0][1] : '');
        return (first + second).toUpperCase();
    }

    function renderAvatar(container, avatar, name) {
        container.innerHTML = '';
        if (avatar) {
            var img = document.createElement('img');
            img.src = avatar;
            img.alt = '';
            container.appendChild(img);
        } else {
            container.textContent = initials(name);
        }
    }

    function initProfile() {
        if (document.querySelector('[data-profile-username]') === null) { return; }

        var user = window.ForumAuth.currentUser();
        var params = new URLSearchParams(window.location.search);
        var viewedName = params.get('name');

        if (!viewedName) {
            // Brak nazwy w adresie — kierujemy na profil bieżącej sesji (lub na logowanie).
            if (user) {
                window.location.replace('/Account/Profile?name=' + encodeURIComponent(user.username));
            } else {
                window.location.replace(loginUrl('/Account/Profile'));
            }
            return;
        }

        var isOwn = user && user.username.toLowerCase() === viewedName.toLowerCase();

        document.querySelectorAll('[data-profile-avatar]').forEach(function (el) {
            if (isOwn) { renderAvatar(el, user.avatar, user.username); }
        });

        if (isOwn) {
            var usernameEl = document.querySelector('[data-profile-username]');
            var emailEl = document.querySelector('[data-profile-email]');
            var joinedEl = document.querySelector('[data-profile-joined]');
            if (usernameEl) { usernameEl.textContent = user.username; }
            if (emailEl) {
                emailEl.innerHTML = '<i class="bi bi-envelope me-1"></i>' + user.email;
            }
            if (joinedEl && user.joinedAt) {
                joinedEl.textContent = new Date(user.joinedAt).toLocaleDateString('pl-PL');
            }
            document.querySelectorAll('[data-profile-rank]').forEach(function (el) {
                el.textContent = user.rank;
            });
        }
    }

    // --- Ustawienia ---------------------------------------------------------

    function hydrateSettingsForm() {
        var form = document.getElementById('settingsAccountForm');
        if (!form) { return; }

        var user = window.ForumAuth.currentUser();
        if (!user) {
            window.location.replace(loginUrl('/Account/Settings'));
            return;
        }

        var display = form.querySelector('[name="DisplayName"]');
        var email = form.querySelector('[name="Email"]');
        if (display) { display.value = user.username; }
        if (email) { email.value = user.email; }

        var personal = document.getElementById('settingsPersonalForm');
        if (personal) {
            var pageSize = personal.querySelector('[name="PageSize"]');
            var idle = personal.querySelector('[name="IdleMinutes"]');
            var theme = personal.querySelector('[name="Theme"]');
            if (pageSize) { pageSize.value = user.settings.pageSize; }
            if (idle) { idle.value = user.settings.idleMinutes; }
            if (theme) { theme.value = window.ForumTheme.current(); }
        }

        document.querySelectorAll('[data-profile-avatar]').forEach(function (el) {
            renderAvatar(el, user.avatar, user.username);
        });
    }

    function initSettingsForms() {
        var accountForm = document.getElementById('settingsAccountForm');
        if (accountForm) {
            accountForm.addEventListener('submit', function (event) {
                event.preventDefault();
                clearErrors(accountForm);
                window.ForumAuth.updateAccount({
                    displayName: accountForm.querySelector('[name="DisplayName"]').value.trim(),
                    email: accountForm.querySelector('[name="Email"]').value.trim()
                }).then(function (result) {
                    if (result.ok) {
                        showSuccess(accountForm, 'Dane konta zostały zapisane.');
                    } else {
                        showErrors(accountForm, result.errors);
                    }
                });
            });
        }

        var passwordForm = document.getElementById('settingsPasswordForm');
        if (passwordForm) {
            passwordForm.addEventListener('submit', function (event) {
                event.preventDefault();
                clearErrors(passwordForm);
                window.ForumAuth.changePassword({
                    currentPassword: passwordForm.querySelector('[name="CurrentPassword"]').value,
                    newPassword: passwordForm.querySelector('[name="NewPassword"]').value,
                    confirmNewPassword: passwordForm.querySelector('[name="ConfirmNewPassword"]').value
                }).then(function (result) {
                    if (result.ok) {
                        passwordForm.reset();
                        showSuccess(passwordForm, 'Hasło zostało zmienione.');
                    } else {
                        showErrors(passwordForm, result.errors);
                    }
                });
            });
        }

        var personalForm = document.getElementById('settingsPersonalForm');
        if (personalForm) {
            personalForm.addEventListener('submit', function (event) {
                event.preventDefault();
                clearErrors(personalForm);

                var pageSize = parseInt(personalForm.querySelector('[name="PageSize"]').value, 10);
                var idle = parseInt(personalForm.querySelector('[name="IdleMinutes"]').value, 10);
                var theme = personalForm.querySelector('[name="Theme"]').value;

                var errors = {};
                if (isNaN(pageSize) || pageSize < 5 || pageSize > 50) {
                    errors.PageSize = 'Liczba wiadomości na stronie musi być z zakresu 5–50.';
                }
                if (isNaN(idle) || idle < 1 || idle > 1440) {
                    errors.IdleMinutes = 'Czas bezczynności musi być z zakresu 1–1440 minut.';
                }
                if (Object.keys(errors).length > 0) {
                    showErrors(personalForm, errors);
                    return;
                }

                window.ForumAuth.saveSettings({ pageSize: pageSize, idleMinutes: idle, theme: theme });
                showSuccess(personalForm, 'Ustawienia personalizacji zostały zapisane.');
            });
        }
    }

    // --- Avatar -------------------------------------------------------------

    var MAX_BYTES = 200 * 1024;   // 200 kB
    var MAX_WIDTH = 512;
    var MAX_HEIGHT = 512;

    function initAvatarSection() {
        var input = document.getElementById('avatarInput');
        if (!input) { return; }

        var errorBox = document.querySelector('[data-form-alert="avatar-error"]');

        function showError(text) {
            if (errorBox) {
                errorBox.textContent = text;
                errorBox.classList.remove('d-none');
            }
        }

        input.addEventListener('change', function () {
            var file = input.files && input.files[0];
            if (!file) { return; }

            var allowed = ['image/jpeg', 'image/png', 'image/webp'];
            if (allowed.indexOf(file.type) < 0) {
                showError('Niedozwolony format pliku — wybierz JPG, PNG lub WEBP.');
                input.value = '';
                return;
            }
            if (file.size > MAX_BYTES) {
                showError('Plik jest za duży — maksymalnie 200 kB (wybrano ' +
                    Math.ceil(file.size / 1024) + ' kB).');
                input.value = '';
                return;
            }

            var reader = new FileReader();
            reader.onload = function () {
                var image = new Image();
                image.onload = function () {
                    if (image.naturalWidth > MAX_WIDTH || image.naturalHeight > MAX_HEIGHT) {
                        showError('Rozdzielczość ' + image.naturalWidth + '×' + image.naturalHeight +
                            ' px przekracza limit 512×512 px.');
                        input.value = '';
                        return;
                    }
                    if (errorBox) { errorBox.classList.add('d-none'); }
                    window.ForumAuth.setAvatar(reader.result);
                    document.querySelectorAll('[data-profile-avatar]').forEach(function (el) {
                        renderAvatar(el, reader.result, window.ForumAuth.currentUser().username);
                    });
                };
                image.onerror = function () {
                    showError('Nie udało się odczytać obrazu.');
                    input.value = '';
                };
                image.src = reader.result;
            };
            reader.readAsDataURL(file);
        });

        var clearButton = document.getElementById('avatarClear');
        if (clearButton) {
            clearButton.addEventListener('click', function () {
                window.ForumAuth.clearAvatar();
                input.value = '';
                document.querySelectorAll('[data-profile-avatar]').forEach(function (el) {
                    var user = window.ForumAuth.currentUser();
                    renderAvatar(el, null, user ? user.username : '');
                });
            });
        }
    }

    // --- Inicjalizacja ------------------------------------------------------

    document.addEventListener('DOMContentLoaded', function () {
        initLoginForm();
        initRegisterForm();
        initProfile();
        hydrateSettingsForm();
        initSettingsForms();
        initAvatarSection();
    });
})();
