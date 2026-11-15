/* ============================================================
   site.js — pętla główna interfejsu:
     * uwidocznianie elementów topbara wg sesji (data-auth-show)
     * przełącznik motywów + stan aktywnej pozycji
     * obsługa wylogowania, inicjały avatara
   ============================================================ */
(function () {
    'use strict';

    function initials(name) {
        if (!name) { return '?'; }
        var parts = String(name).trim().split(/\s+/);
        var first = parts[0] ? parts[0][0] : '?';
        var second = parts.length > 1 ? parts[parts.length - 1][0] : (parts[0] && parts[0].length > 1 ? parts[0][1] : '');
        return (first + second).toUpperCase();
    }

    function renderAuthState(user) {
        document.querySelectorAll('[data-auth-show]').forEach(function (el) {
            var scope = el.getAttribute('data-auth-show');
            var visible;
            if (scope === 'guest') {
                visible = !user;
            } else if (scope === 'user') {
                visible = !!user;
            } else if (scope === 'admin') {
                visible = !!user && user.role === 'admin';
            } else {
                visible = true;
            }
            el.classList.toggle('d-none', !visible);
        });

        document.querySelectorAll('[data-auth-username]').forEach(function (el) {
            el.textContent = user ? user.username : '';
        });

        document.querySelectorAll('[data-auth-avatar]').forEach(function (el) {
            el.innerHTML = '';
            if (user && user.avatar) {
                var img = document.createElement('img');
                img.src = user.avatar;
                img.alt = '';
                el.appendChild(img);
            } else {
                el.textContent = user ? initials(user.username) : '';
            }
        });
    }

    function renderThemeState() {
        var current = window.ForumTheme.current();
        document.querySelectorAll('[data-theme-set]').forEach(function (el) {
            el.classList.toggle('is-active', el.getAttribute('data-theme-set') === current);
        });
    }

    function bindThemeMenu() {
        document.querySelectorAll('[data-theme-set]').forEach(function (el) {
            el.addEventListener('click', function (event) {
                event.preventDefault();
                window.ForumTheme.set(el.getAttribute('data-theme-set'));
                renderThemeState();
            });
        });
        document.addEventListener('themechange', renderThemeState);
    }

    function bindLogout() {
        document.querySelectorAll('[data-logout]').forEach(function (el) {
            el.addEventListener('click', function (event) {
                event.preventDefault();
                window.ForumAuth.logout();
                window.location.href = '/';
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        window.ForumAuth.onAuthChange(renderAuthState);
        renderAuthState(window.ForumAuth.currentUser());
        renderThemeState();
        bindThemeMenu();
        bindLogout();
        document.documentElement.classList.add('js-ready');
    });
})();
