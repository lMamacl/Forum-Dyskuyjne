# Forum Dyskusyjne

Projekt zaliczeniowy z przedmiotu **Aplikacje webowe w ASP MVC** — internetowe forum
dyskusyjne oparte o ASP.NET Core MVC (.NET 10) i Bootstrap 5.

## Uruchomienie

```bash
cd "Forum Dyskuyjne"
dotnet run --urls http://localhost:5084
```

Aplikacja startuje z kompletnymi danymi demonstracyjnymi — nie jest wymagana baza danych
ani ręczne wprowadzanie czegokolwiek.

| Rola | E-mail | Hasło |
|---|---|---|
| Administrator | `admin@forum.pl` | `Admin123!` |
| Moderator | `mod@forum.pl` | `Mod123!` |
| Użytkownik | `user@forum.pl` | `User123!` |

## Dokumentacja projektowa

- [`docs/01-analiza-wymagan.md`](docs/01-analiza-wymagan.md) — analiza wymagań, punktacja
  (45 pkt), selekcja funkcjonalności, architektura i ścieżki migracji.
- [`docs/02-plan-pracy-i-wdrozenia.md`](docs/02-plan-pracy-i-wdrozenia.md) — podział pracy,
  kontrakty wspólne, kolejność wdrożenia, checklista dema.

## Aktualny zakres (etap 1)

Widoki i formularze całego forum z działającą logiką demonstracyjną:

- layout (topbar ze skórkami, panelu użytkownika i zakładkami „Wątki”/„Fora”, stopka),
- strona główna z ogłoszeniami i statystykami,
- **widok domyślny: przegląd kategorii i forów** z paskiem wyszukiwarki
  wszystkich wątków,
- lista wątków (`/Forum/Threads`): filtry — fraza, **forum**, **kategoria (tag)**,
  sortowanie; paginacja,
- jeden wątek należy do jednego forum (wybór w formularzu tworzenia);
  kategorie = tagi: wątek może mieć ich kilka (min. jeden), słownik tworzy
  administrator,
- strona wątku z odpowiedziami i edytorem inline,
- tworzenie/edycja/usuwanie wątków i wiadomości,
- rejestracja, logowanie, profil, ustawienia (localStorage),
- panel administracyjny: użytkownicy, kategorie, fora, ogłoszenia,
- trzy motywy interfejsu.

Etap 2 (baza danych + Identity) i etap 3 (funkcje dodatkowe) — plan w dokumencie 02.
