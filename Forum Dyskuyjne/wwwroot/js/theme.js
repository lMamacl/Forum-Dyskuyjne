/* ============================================================
   theme.js — przełączanie skórek interfejsu (3 warianty)
   Wybór zapisywany w localStorage; zastosowanie przed załadowaniem
   treści odbywa się w inline skrypcie w _Layout (anty-migotanie).
   ============================================================ */
(function () {
    'use strict';

    var STORAGE_KEY = 'forum.theme';
    var THEMES = ['light', 'dark', 'ocean'];

    function readStored() {
        try {
            var value = window.localStorage.getItem(STORAGE_KEY);
            return THEMES.indexOf(value) >= 0 ? value : 'light';
        } catch (e) {
            return 'light';
        }
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.setAttribute('data-bs-theme', theme === 'dark' ? 'dark' : 'light');
        document.dispatchEvent(new CustomEvent('themechange', { detail: theme }));
    }

    window.ForumTheme = {
        themes: THEMES,
        current: readStored,
        set: function (theme) {
            if (THEMES.indexOf(theme) < 0) { return; }
            try { window.localStorage.setItem(STORAGE_KEY, theme); } catch (e) { /* prywatny tryb */ }
            applyTheme(theme);
        }
    };

    applyTheme(readStored());
})();
