/* ============================================================
   admin.js — zachowania panelu administracyjnego:
     * strażnik dostępu: bez sesji administratora przekierowanie na stronę główną
       (w etapie 2 zastąpiony atrybutem [Authorize(Roles="Admin")])
     * potwierdzenie operacji usuwania (form[data-confirm])
   ============================================================ */
(function () {
    'use strict';

    function isGuardedPage() {
        return document.querySelector('.admin-tabs') !== null;
    }

    document.addEventListener('DOMContentLoaded', function () {
        if (isGuardedPage() && !window.ForumAuth.isAdmin()) {
            window.location.replace('/');
            return;
        }

        document.querySelectorAll('form[data-confirm]').forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!window.confirm(form.getAttribute('data-confirm'))) {
                    event.preventDefault();
                }
            });
        });
    });
})();
