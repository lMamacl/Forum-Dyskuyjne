/* ============================================================
   forum.js — zachowania widoków forum:
     * wypełnianie pola AuthorName z sesji (auth.js) przed wysłaniem formularzy
     * wymaganie logowania przy tworzeniu wątku i odpowiadaniu
     * ujawnianie przycisków moderacji (admin / moderator) i checkboxa „przyklej”
     * potwierdzanie operacji nieodwracalnych (usuwanie)
   ============================================================ */
(function () {
    'use strict';

    function currentUsername() {
        var user = window.ForumAuth.currentUser();
        return user ? user.username : '';
    }

    function fillAuthorFields() {
        var name = currentUsername();
        document.querySelectorAll('[data-author-name]').forEach(function (el) {
            el.value = name;
        });
    }

    function loginUrl() {
        return '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname + window.location.search);
    }

    function guardAuth(event) {
        if (!window.ForumAuth.isLoggedIn()) {
            event.preventDefault();
            event.stopPropagation();
            window.location.href = loginUrl();
            return false;
        }
        return true;
    }

    function applyRoleVisibility() {
        var moderator = window.ForumAuth.isModerator();
        var admin = window.ForumAuth.isAdmin();

        document.querySelectorAll('[data-mod-only]').forEach(function (el) {
            el.style.display = moderator ? '' : 'none';
        });
        document.querySelectorAll('[data-admin-only]').forEach(function (el) {
            el.classList.toggle('d-none', !admin);
        });
    }

    function bindAuthGuards() {
        // Linki i przyciski wymagające sesji (np. „Nowy wątek”)
        document.querySelectorAll('[data-requires-auth]').forEach(function (el) {
            el.addEventListener('click', guardAuth);
        });

        // Formularze: tworzenie wątku i odpowiedź
        ['#createThreadForm', '#replyForm'].forEach(function (selector) {
            var form = document.querySelector(selector);
            if (form) {
                form.addEventListener('submit', guardAuth);
            }
        });
    }

    function bindConfirmations() {
        document.querySelectorAll('form[data-confirm]').forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!window.confirm(form.getAttribute('data-confirm'))) {
                    event.preventDefault();
                }
            });
        });
    }

    function init() {
        fillAuthorFields();
        applyRoleVisibility();
        bindAuthGuards();
        bindConfirmations();
    }

    document.addEventListener('DOMContentLoaded', init);
    window.ForumAuth.onAuthChange(function () {
        fillAuthorFields();
        applyRoleVisibility();
    });
})();
