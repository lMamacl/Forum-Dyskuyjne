# Plan pracy i wdrożenia — Forum dyskusyjne

**Dokument operacyjny do pracy równoległej w etapie 1 (widoki i formularze).**
Uzupełnienie analizy: `docs/01-analiza-wymagan.md`.
Aktualizowany na bieżąco — ostatnia aktualizacja: 15.11.2026.

---

## 1. Status skorupy (co jest gotowe)

Skorupa rozwiązania jest kompletna i działa end-to-end bez bazy danych:

| Obszar | Stan |
|---|---|
| Layout: topbar (nawigacja, panel użytkownika, 3 motywy, ukryty panel admina) + stopka | gotowe |
| Strona główna: powitanie, statystyki, ogłoszenia, ostatnie wątki | gotowe |
| Kategorie i fora: lista z uprawnieniami anonimowymi, moderatorami, statystykami | gotowe |
| Lista wątków: paginacja, wyszukiwarka, badge „przyklejony”, statystyki wiersza | gotowe |
| Strona wątku: wiadomość inicjująca, odpowiedzi z paginacją, edytor inline, licznik odsłon | gotowe |
| Tworzenie wątku, edycja/usuwanie wiadomości (z perspektywą moderacji) | gotowe |
| Rejestracja i logowanie z walidacją po stronie klienta (localStorage + `auth.js`) | gotowe |
| Profil: awatar, ranga, statystyki, ostatnie wypowiedzi | gotowe |
| Ustawienia: konto, hasło, liczba stron, bezczynność, motyw, awatar (z walidacją) | gotowe |
| Panel admina: pulpit, użytkownicy (rola/ranga/ban), kategorie, fora (uprawnienia, moderatorzy), ogłoszenia | gotowe |
| Dane demonstracyjne (3 konta, 3 kategorie, 6 forów, 27 wątków, odpowiedzi, 2 ogłoszenia) | gotowe |
| Skrypty walidacji (DataAnnotations + jQuery Unobtrusive) | gotowe |
| Ikony Bootstrap Icons lokalnie (bez CDN) | gotowe |

Aplikacja startuje bez żadnych dodatkowych czynń (wymaganie z zasad zaliczenia):
`dotnet run` → strona główna z pełnymi danymi.

---

## 2. Uruchomienie projektu

```bash
cd "Forum Dyskuyjne"
dotnet run --urls http://localhost:5084
```

Przeglądarka: `http://localhost:5084`

**Konta demonstracyjne** (logowanie przez formularz na stronie `/Account/Login`):

| Rola | E-mail | Hasło |
|---|---|---|
| Administrator | `admin@forum.pl` | `Admin123!` |
| Moderator | `mod@forum.pl` | `Mod123!` |
| Użytkownik | `user@forum.pl` | `User123!` |

Rejestracja nowego konta również działa (zapis do `localStorage`, hasło hashowane SHA-256).

---

## 3. Struktura projektu i mapa własności

```
Forum Dyskuyjne/
├── Controllers/
│   ├── HomeController.cs .............. wspólne
│   ├── ForumController.cs ............. MACIEJ
│   ├── AccountController.cs ........... ZNAJOMY
│   └── AdminController.cs ............. ZNAJOMY
├── Models/
│   ├── Domain/ ......................... wspólne (encje — zgłaszać zmiany!)
│   └── ViewViewModels/ ................ podział jak niżej
├── Services/
│   ├── IForumRepository.cs ............ wspólne (kontrakt danych)
│   ├── InMemoryForumRepository.cs ..... wspólne
│   ├── SeedData.cs .................... wspólne
│   └── PagedResult.cs ................. wspólne
├── Views/
│   ├── Shared/  (_Layout, _Topbar, _Footer, _Pagination, _AdminTabs, Error) ... wspólne
│   ├── Home/ ........................... wspólne
│   ├── Forum/ .......................... MACIEJ
│   ├── Account/ ....................... ZNAJOMY
│   └── Admin/ .......................... ZNAJOMY
└── wwwroot/
    ├── css/site.css .................... wspólne (zmiany zgłaszać!)
    └── js/
        ├── site.js, theme.js, auth.js .. wspólne (kontrakty — patrz sekcja 5)
        ├── forum.js .................... MACIEJ
        └── account.js, admin.js ........ ZNAJOMY
docs/
├── 01-analiza-wymagan.md ............... analiza, punktacja, architektura
└── 02-plan-pracy-i-wdrozenia.md ........ ten dokument
```

### Podział zadań w etapie 1

| Osoba | Zakres | Punkty za które odpowiada |
|---|---|---|
| **Maciej** | treści forum: lista wątków, strona wątku, tworzenie, edycja, wyszukiwarka, paginacja, statystyki wątków | 1, 7, 8 (część), 13, 14 |
| **Znajomy** | konta: logowanie, rejestracja, profil, ustawienia; panel admina: użytkownicy, kategorie, fora, ogłoszenia | 2, 3, 4, 9, 11 (część), 12 (część) |
| **Wspólne** | layout, modele, repozytorium, motywy, sejsja (`auth.js`), routing, PR-y do mastera | — |

Uwaga: skorupa zawiera **działające makiety** całej reszty — zadaniem właściciela obszaru jest je przetestować, dopracować i dopisać brakujące przypadki graniczne, a nie budować od zera.

---

## 4. Kolejność prac (etap 1 — domknięcie)

1. **Synchronizacja** — obaj: `git pull`, uruchomienie projektu, przejście po wszystkich trasach z sekcji 7 (checklista dema).
2. **Przegląd własnej części** — każdy robi 30-minutowy przegląd swoich plików, notuje braki.
3. **Praca równoległa** (równocześnie, bez dotykania plików wspólnych):
   - Maciej: `Views/Forum/*`, `ForumController`, `forum.js` — poprawki UX, brakujące przypadki (np. pusty wynik wyszukiwania, długie tytuły).
   - Znajomy: `Views/Account/*`, `Views/Admin/*`, `AccountController`, `AdminController`, `account.js`, `admin.js` — testy formularzy, walidacja, poprawki list admina.
4. **Cross-review** — każdy przegląda kod drugiego (15 min), komentarze w kommitach lub issue'ach.
5. **Integracja** — merge do `master`, wspólny przebieg checklisty (sekcja 7).
6. **Demo wewnętrzne** — symulacja prezentacji prowadzącemu na podstawie sekcji 8 analizy; lista poprawek → zamknięcie.
7. **Przed terminem cząstkowym** — każdy przygotowuje opis swoich funkcjonalności (2–3 zdania na funkcję) pod dokumentację.

### Konwencje git

- Branch roboczy: `etap1/<imię>/<temat>` — np. `etap1/maciej/wyszukiwarka`, `etap1/znajomy/awatary`.
- Commit: czasownik w czasie przeszłym + zakres — „Dodano paginację wątków”, „Poprawiono walidację hasła”.
- Przed merge: `dotnet build` bez błędów + ręczny przegląd zmienionych widoków.
- Pliki wspólne (sekcja 3) — zmieniać wyłącznie po uzgodnieniu (komentarz na Discordzie/issue).
- Małe commity (max ~300 linii) — łatwiej reviewować i cofać.

---

## 5. Kontrakty wspólne (muszą działać u obu)

### 5.1. `auth.js` — API sesji klienta

```js
await ForumAuth.ready                          // seed kont demo zakończony
await ForumAuth.register({username, email, password, confirmPassword}) -> {ok, errors}
await ForumAuth.login({email, password, remember})                     -> {ok, errors}
ForumAuth.logout()
ForumAuth.currentUser()   -> {username, email, role, rank, joinedAt, avatar, settings} | null
ForumAuth.isLoggedIn() / isModerator() / isAdmin()
await ForumAuth.updateAccount({displayName, email})                    -> {ok, errors}
await ForumAuth.changePassword({currentPassword, newPassword, confirmNewPassword}) -> {ok, errors}
ForumAuth.saveSettings({pageSize, idleMinutes, theme})   // pageSize dodatkowo → cookie
ForumAuth.setAvatar(dataUrl) / clearAvatar() / getAvatar()
ForumAuth.onAuthChange(listener)               // wywoływane przy login/logout/zmianie danych
```

Rola w sesji: `'admin' | 'moderator' | 'user'` — zapisywana przy rejestracji konta demo.
W etapie 2 API pozostaje bez zmian; implementacja przechodzi na HTTP + Identity.

### 5.2. Atrybuty danych w widokach

| Atrybut | Znaczenie |
|---|---|
| `data-auth-show="guest\|user\|admin"` | widoczność elementu topbara wg sesji |
| `data-auth-username`, `data-auth-avatar` | podmiana nazwy/awatara przez `site.js` |
| `data-requires-auth` | element wymaga logowania (klik → przekierowanie) |
| `data-mod-only` | widoczne dla admina/moderatora (`forum.js`) |
| `data-admin-only` | widoczne wyłącznie dla admina (`forum.js`) |
| `data-author-name` | ukryte pole wypełniane nazwą z sesji przed POST |
| `data-confirm="..."` | potwierdzenie przed wysłaniem formularza |
| `data-theme-set="light\|dark\|ocean"` | pozycja przełącznika motywów |
| `data-form-alert`, `data-auth-errors` | miejsce na komunikaty JS w formularzach |

### 5.3. Wzorce widoków (powtarzać identycznie)

- Ostrzeżenia po POST: `TempData["Success"]` / `TempData["Error"]` → `.alert.alert-success|danger.alert-site`.
- Walidacja: `asp-validation-summary="ModelOnly"` + `asp-validation-for` + skrypt `_ValidationScriptsPartial`.
- Paginacja: deklaracja `var pager = new PaginationViewModel {...}` w `@{ }`, następnie `<partial name="_Pagination" model="pager" />`.
- Formularze admina: pola z prefiksem `Form.*`, parametr akcji nazany `Form`.
- Prefiks `Reply.*` w formularzu odpowiedzi → parametr akcji `Reply`.
- Cookie paginacji: `forum.pageSize` (zakres 5–50, ustawiane w Ustawieniach).

### 5.4. Trasy

| Trasa | Kontroler |
|---|---|
| `GET /` | Home/Index |
| `GET /Forum` | Forum/Index (kategorie i fora) |
| `GET /Forum/Threads?forumId=&page=&q=` | Forum/Threads |
| `GET /Forum/Thread?id=&page=` | Forum/Thread |
| `GET,POST /Forum/CreateThread` | Forum/CreateThread |
| `GET,POST /Forum/EditMessage`, `POST /Forum/DeleteMessage` | Forum |
| `POST /Forum/Reply` | Forum/Reply |
| `GET /Account/Login\|Register\|Profile\|Settings` | Account |
| `GET /Admin`, `/Admin/Users`, `/Admin/Categories`, `/Admin/Forums`, `/Admin/Announcements` | Admin |
| `POST /Admin/EditUser\|SaveCategory\|DeleteCategory\|SaveForum\|DeleteForum\|SaveAnnouncement\|DeleteAnnouncement` | Admin |

### 5.5. Lista `TODO (etap 2)` w kodzie

Wyszukaj w repozytorium frazę `TODO (etap 2)` — są to jawne punkty do podpięcia
uwierzytelnienia i uprawnień serwerowych. Nie kasować — oznaczają miejsca,
gdzie dziś brakuje weryfikacji tożsamości.

---

## 6. Plan etapu 2 (baza danych) — kolejność kroków

Kolejność ograniczała ryzyko „przebudowy wszystkiego naraz”; każdy krok kończy się działającą aplikacją.

1. **Pakiety i DbContext** — `Microsoft.EntityFrameworkCore.SqlServer`, kontekst, encje z `Models/Domain`, `Program.cs` (connection string w `appsettings.json`).
2. **Migracje + seed** — `dotnet ef migrations add Init`, `dotnet ef database update`; seed przeniesiony z `SeedData.cs` do metody `EnsureSeeded` (wymóg: aplikacja startuje z danymi bez ręcznego wpisywania).
3. **`EfForumRepository : IForumRepository`** — implementacja metod interfejsu (zapytania LINQ); podmiana rejestracji w `Program.cs` (`AddSingleton` → `AddScoped`). **Kontrolery i widoki bez zmian.** Weryfikacja checklisty z sekcji 7.
4. **ASP.NET Identity** — pakiet, rejestracja w DI, `ApplicationUser` (po Id użytkownika z `UserAccounts`), strony Identity zastąpione naszymi `Account/*` (zmiana formularzy na POST do kontrolera). `auth.js` przestaje trzymać konta — zastępuje je obsługa cookies serwera; `data-auth-show` mogą przełączać części w `_Topbar` z `User.Identity`.
5. **Uprawnienia serwerowe** — zaimplementować wszystkie `TODO (etap 2)`: `[Authorize]`, `[Authorize(Roles="Admin")]`, sprawdzenie `ModeratorIds`/autora przy edycji i usuwaniu, `AllowAnonymousView/Post` przy wejściu na forum i przy POST wątku.
6. **Stronicowanie i ustawienia konta** — `PageSize`/`IdleMinutes` z konta zamiast cookie.
7. **Funkcje z warstwy B/C** wg `docs/01` sekcja 3 (kolejność: operatory wyszukiwania → zgłoszenia → emotikony → rangi auto → załączniki → słownik → reset hasła → HTML → i18n).
8. **Dokumentacja** — PDF/ODT/DOC ze strona tytułową: opis projektu, instrukcja użytkownika (checklista z sekcji 8 analizy), opis zrealizowanych funkcjonalności i punktacja.

---

## 7. Checklista dema dla prowadzącego

Uruchom aplikację i przejdź po kolei (~10 minut):

1. **Strona główna** — statystyki, ogłoszenia, ostatnie wątki; przyciski logowania dla gościa.
2. **Wątki (topbar)** — kategorie z forami; widoczne badge uprawnień anonimowych i moderatorzy.
3. **Forum „Programowanie”** — 12 wątków → paginacja, dwa przyklejone na górze, statystyki odpowiedzi/odsłon.
4. **Wyszukiwanie** — fraza „EF” w tym forum zawęża listę; wyczyść ikoną X.
5. **Nowy wątek jako gość** — klik przekierowuje do logowania.
6. **Logowanie jako `admin@forum.pl` / `Admin123!`** — topbar pokazuje nazwę; dropdown ma „Panel administracyjny”.
7. **Nowy wątek** — formularz z walidacją (spróbuj krótkiego tytułu), po zapisie wątek widoczny; checkbox „przyklej” widoczny tylko dla admina.
8. **Strona wątku** — licznik odsłon rośnie, odpowiedź dodaje się na dole; przyciski Edytuj/Usuń widoczne dla admina; usuwanie pyta o potwierdzenie.
9. **Profil** — awatar, ranga, statystyki, ostatnie wypowiedzi.
10. **Ustawienia** — zmień liczbę wiadomości na stronie (np. 5) → wróć na listę wątków: mniej pozycji; zmień motyw (3 warianty); zmień hasło; wczytaj avatar z limitem 200 kB i 512 px (próbuj większego — komunikat błędu).
11. **Wylogowanie po bezczynności** — ustaw 1 minutę w profilu, poczekaj → komunikat na ekranie logowania.
12. **Panel admina** — pulpit ze statystykami; użytkownicy: zmień rangę; kategorie: dodaj/edytuj/usuń (spróbuj usunąć kategorię z forami — błąd); fora: dodaj forum z kategorią i moderatorami; ogłoszenia: dodaj → widoczne na stronie głównej.
13. **Trzeci motyw** — „Ocean” w topbarze; wybór zapamiętany po odświeżeniu.

---

## 8. Definicja gotowości (Definition of Done) dla zadania

Zadanie (widok/funkcja) uznajemy za zamknięte, gdy:

- [ ] `dotnet build` — 0 błędów, 0 nowych ostrzeżeń.
- [ ] Trasa działa w przeglądarce (nie tylko w curl).
- [ ] Formularz ma walidację po stronie klienta **i** atrybuty na ViewModelu.
- [ ] Komunikaty sukcesu/błędu widoczne dla użytkownika.
- [ ] Brak niedziałających linków i pustych przycisków w danym widoku.
- [ ] Zmiany w plikach wspólnych omówione z drugą osobą.
- [ ] Kod skomitowany z opisem w konwencji z sekcji 4.
