# Analiza wymagań projektu — Forum dyskusyjne

**Przedmiot:** Aplikacje webowe w ASP MVC (ASP.NET Core MVC, .NET 10)
**Repozytorium:** https://github.com/lMamacl/Forum-Dyskuyjne
**Data analizy:** 15.11.2026
**Dokument roboczy:** wersja 1.0

---

## 1. Podsumowanie

Projekt obejmuje internetowe forum dyskusyjne z pełnymi funkcjonalnościami społecznościowymi i administracyjnymi. Do zdobycia jest maksymalnie **45 punktów**, przy czym łączna pula wymaganych funkcjonalności wynosi **64 punkty** — oznacza to konieczność **wyboru** zakresu: wszystkiego naraz zrealizować się nie da i nie warto.

Projekt przechodzi przez **3 terminy cząstkowe**:

| Termin | Zakres | Cel |
|---|---|---|
| T1 | Formularze i widoki | Kompletny, klikalny interfejs z walidacją — przedstawienie prowadzącemu |
| T2 | Baza danych | Trwałe przechowywanie danych, uwierzytelnianie serwerowe, logika biznesowa |
| T3 | Ostateczny | Funkcje dodatkowe, dokumentacja (PDF/ODT/DOC), pakiet zasobów |

Dokument opisuje: selekcję funkcjonalności, architekturę, mapę widoków oraz ryzyka.
Podział pracy i kolejność wdrożenia: `docs/02-plan-pracy-i-wdrozenia.md`.

---

## 2. Tabela funkcjonalności i punktacja

Ocena trudności: **N** — niska (do zrobienia w kilkadziesiąt minut), **Ś** — średnia (kilka godzin), **W** — wysoka (wymaga przemyślanego rozwiązania, np. parser, integracja z pocztą).

| Lp. | Funkcjonalność | Pkt | Trudn. | Termin | Status w skorupie T1 |
|---|---|---|---|---|---|
| 1 | Tworzenie wątków i dopisywanie wiadomości | 4 | N | T1+T2 | Widoki + formularze + akcje in-memory |
| 2 | Rejestracja, profil, logowanie | 4 | Ś | T1+T2 | Formularze z walidacją, sesja w localStorage |
| 3 | Panel administracyjny (wiadomości i użytkownicy) | 4 | Ś | T1+T2 | Dashboard + widok użytkownierów |
| 4 | Kategorie i fora (tylko admin, forum = 1 kategoria) | 4 | N | T1+T2 | Widoki listy + formularze CRUD |
| 5 | Uprawnienia anonimowe per forum (podgląd / pisanie) | 2 | N | T2 | Model + pola w formularzu admina |
| 6 | Moderatorzy wielu for, edycja/usuwanie wiadomości | 4 | Ś | T1+T2 | Model relacji + przyciski w widoku |
| 7 | Statystyki (użytkownicy, wątki/wiadomości, odsłony/odpowiedzi) | 2 | N | T1 | Gotowe — dane z seed, liczniki |
| 8 | Wątki przyklejone (tylko admin) | 2 | N | T1+T2 | Wyświetlanie + badge, checkbox dla admina |
| 9 | Ogłoszenia administracyjne na stronie głównej | 2 | N | T1+T2 | Sekcja na Home + formularz w adminie |
| 10 | Wiadomości prywatne (odczyt, usuwanie) | 2 | Ś | T2/T3 | — |
| 11 | Automatyczne wylogowanie po bezczynności | 2 | N | T1 | Timer w `auth.js`, ustawienie w profilu |
| 12 | Awatary (limit rozdzielczości i wielkości w kB) | 2 | N | T1 | Upload z walidacją po stronie klienta |
| 13 | Stronicowanie list wiadomości (ustawienie w profilu) | 2 | N | T1 | Paginacja serwerowa + cookie z ustawieniem |
| 14 | Wyszukiwanie słów kluczowych w wiadomościach forum | 2 | N | T1 | Formularz GET na stronie wątków |
| 15 | Operatory AND / OR / NOT przy wyszukiwaniu | 2 | Ś | T2/T3 | Rozszerzenie punktowe parsera |
| 16 | Załączniki (limit liczby i wielkości) | 2 | Ś | T2/T3 | — |
| 17 | Zgłoszenie wiadomości do moderacji | 2 | N | T2/T3 | — |
| 18 | Słownik słów zakazanych (admin) | 4 | Ś | T3 | — |
| 19 | Emotikony w treści wiadomości | 2 | N | T2/T3 | — |
| 20 | HTML z listą dozwolonych znaczników (sanitizer) | 4 | W | T3 | — |
| 21 | Rangi automatyczne + ręczna zmiana (admin) | 2 | N | T2 | Kolumna rank w panelu admina |
| 22 | Bezpieczna procedura przypominania i zmiany hasła | 2 | W | T2/T3 | Wymaga wysyłki e-mail |
| 23 | Skórki interfejsu (2 istotne + 1 trzecia) | 3 | N | **T1** | Gotowe — 3 motywy w topbarze |
| 24 | Dodatkowe wersje językowe (2 pkt + 1 pkt) | 3 | W | T3 | — |
| | **RAZEM w puli** | **64** | | | **cel: 45 pkt** |

---

## 3. Rekomendowany wybór do 45 punktów

### Warstwa A — rdzeń obowiązkowy (28 pkt)

Funkcje, bez których forum nie istnieje i których nie da się pominąć w żadnym terminie:

`1` (4) + `2` (4) + `3` (4) + `4` (4) + `5` (2) + `6` (4) + `7` (2) + `8` (2) + `9` (2) = **28 pkt**

### Warstwa B — dodatki o wysokim stosunku punktów do nakładu pracy (17 pkt)

| Funkcja | Pkt | Dlaczego warto |
|---|---|---|
| `13` Stronicowanie + ustawienie w profilu | 2 | Paginacja i tak jest potrzebna — wystarczy odczyt cookie |
| `14` Wyszukiwanie w forum | 2 | Formularz GET + jeden zapytanie do repozytorium |
| `11` Wylogowanie po bezczynności | 2 | Ok. 20 linii JS w `auth.js` |
| `19` Emotikony | 2 | Zamiana sekwencji tekstowej na obrazki SVG przy renderowaniu |
| `21` Rangi (auto + ręczna) | 2 | Progi w repozytorium + dropdown w panelu admina |
| `15` Operatory AND/OR/NOT | 2 | Parser zapytania — naturalne rozszerzenie `14` |
| `17` Zgłoszenia do moderacji | 2 | Formularz + kolejka w panelu admina |
| `23` Skórki (3 warianty) | 3 | **Już zrealizowane w skorupie** |
| | **17 pkt** | |

**Razem A + B = 45 pkt.**

### Warstwa C — rezerwa (wybrać 0–19 pkt w zależności od czasu)

Priorytet wg stosunku punktów do pracy:

1. `12` Awatary (2 pkt) — **zaimplementowane w skorupie** (walidacja typu, wymiarów, rozmiaru w kB)
2. `10` Wiadomości prywatne (2 pkt) — skrzynka: lista, odczyt, usuwanie
3. `18` Słownik słów zakazanych (4 pkt) — lista fraz + odrzucanie wiadomości przy zapisie
4. `16` Załączniki (2 pkt) — limit liczby i wielkości pliku
5. `22` Reset hasła (2 pkt) — wymaga konta pocztowego (lub dziennika linków w wersji demonstracyjnej)
6. `20` HTML + dozwolone znaczniki (4 pkt) — najtrudniejsze: lista tagów w adminie + sanityzacja
7. `24` Języki (3 pkt) — pliki zasobów + przełącznik kultury

**Strategia:** planować 48–50 pkt, żeby odjąć ewentualne punkty za niekompletne elementy i mimo to zamknąć się na 45.

---

## 4. Architektura rozwiązania

### 4.1. Warstwy

```
Przeglądarka
   │  formularze (Razor + walidacja jQuery Unobtrusive)
   ▼
Controllers (Home, Forum, Account, Admin)
   │  wiąże ViewModel ←→ widok, walidacja ModelState
   ▼
Services / IForumRepository          ← interfejs (kontrakt warstwy danych)
   │
   ├── InMemoryForumRepository (T1)  ← dane demonstracyjne + zapisy sesji roboczej
   └── EfForumRepository      (T2)   ← docelowa implementacja, EF Core + SQL Server
```

Zasady:

- **MVC bez kombinowania** — każdy widok ma kontroler i ViewModel; formularze korzystają z tag helperów (`asp-for`, `asp-validation-for`) i walidacji po obu stronach.
- **Jeden punkt wymiany danych** — cała treść forum (kategorie, fora, wątki, wiadomości, ogłoszenia, statystyki) przechodzi przez `IForumRepository`. W T1 implementacja jest in-memory z danymi z seed; w T2 podmiana implementacji na EF Core **bez zmian w kontrolerach i widokach**.
- **Konta i sesja w T1 po stronie klienta** (`localStorage`, moduł `auth.js`) — zgodnie z ustaleniem, że na pierwszy termin nie budujemy warstwy serwerowej kont. Kontrakt API jest tak zaprojektowany, aby w T2 podmienić implementację na ASP.NET Identity bez zmian w widokach (patrz 4.3).
- **ViewModely jako kontrakty widoku** — formularze nie przyjmują encji domenowych bezpośrednio; walidacja siedzi w atrybutach na ViewModelach (DataAnnotations).

### 4.2. Modele domenowe (T1 — bez zależności od bazy)

| Encja | Kluczowe pola | Uwagi |
|---|---|---|
| `Category` | Name, Description, Position | Kategoria zawiera wiele forów |
| `Forum` | Name, Description, CategoryId, AllowAnonymousView, AllowAnonymousPost, ModeratorIds | **Forum należy do dokładnie jednej kategorii** (wymaganie 4) |
| `ForumThread` | ForumId, Title, Description, Content, AuthorName, IsPinned, ViewCount, CreatedAt | Wątek = wiadomość inicjująca + odpowiedzi |
| `ForumMessage` | ThreadId, AuthorName, Content, CreatedAt, EditedAt | Odpowiedzi; edytuje autor, moderator, admin |
| `UserAccount` | Username, Email, Role, Rank, MessageCount, IsActive | Konto zarządzane w adminie (źródło listy użytkowników) |
| `Announcement` | Title, Content, CreatedAt, AuthorName | Ogłoszenia na stronie głównej |

Relacje wiele-do-wielu (forum ↔ moderatorzy) reprezentowane jako `List<int> ModeratorIds` w T1 — w T2 tabela `ForumModerators`.

### 4.3. Ścieżki migracji (co się zmienia w T2/T3, co zostaje)

| Element | T1 (skorupa) | T2 (baza danych) | Zmiana w widokach |
|---|---|---|---|
| Treść forum | `InMemoryForumRepository` + seed | `EfForumRepository` + migracje | brak |
| Konta / sesja | `auth.js` + `localStorage` | ASP.NET Identity + cookies | brak (zmiana tylko w `auth.js` → wywołania HTTP) |
| Hasła | SHA-256 w przeglądarce | Identity (PBKDF2) | brak |
| Uprawnienia | wykrywanie roli przez JS | atrybuty `[Authorize]` + filtry | brak |
| Stronicowanie | cookie `forum.pageSize` | ustawienie konta w bazie | brak |
| Wyszukiwanie | proste „wszystkie słowa" | parser AND/OR/NOT | brak |
| Emotikony / HTML | tekst surowy | filtr + sanitizer | minimalne (jedna metoda renderu) |

### 4.4. Decyzje projektowe (uzasadnienie)

1. **Edytor odpowiedzi inline na stronie wątku**, osobny widok tylko do tworzenia wątku — mniej tras, mniej widoków, naturalny UX forów; formularz i tak jest pełnym formularzem MVC z walidacją.
2. **Stronicowanie po stronie serwera** (`?page=`) — standardowy, łatwy do wytłumaczenia wzorzec; numeracja i linki `asp-action` z zachowaniem parametrów.
3. **Wyszukiwarka jako formularz GET na stronie listy wątków** — wymaganie dotyczy wyszukiwania w danym forum; parametr `q` przepada przez kontroler do repozytorium. Operatory T2 = parser wejścia.
4. **Trzy skórki na zmiennych CSS** (`--accent`, `--surface`, ...) + `data-theme` na `<html>`; motyw ciemny dokleja `data-bs-theme="dark"`, żeby komponenty Bootstrapa 5.3 też się przebarwiały. Wybór zapisywany w `localStorage`, odczytywany synchronicznie w `<head>` (bez migotania).
5. **Statystyki liczone w repozytorium** (nie w widoku) — w T2 ta sama metoda zapyta bazę.
6. **Licznik odsłon** inkrementowany przy wejściu na wątek (in-memory) — w T2 jeden `UPDATE`.

---

## 5. Mapa widoków i tras

| Trasa | Widok | Rolownica | Właściciel (T1) |
|---|---|---|---|
| `GET /` | `Home/Index` | powitanie, statystyki, ogłoszenia, ostatnie wątki | wspólny |
| `GET /Forum` | `Forum/Index` | kategorie → fora ze statystykami i uprawnieniami | wspólny |
| `GET /Forum/Threads?forumId=&page=&q=` | `Forum/Threads` | lista wątków, paginacja, wyszukiwarka, „nowy wątek" | **Ty** |
| `GET /Forum/Thread?id=&page=` | `Forum/Thread` | wiadomość inicjująca + odpowiedzi + formularz odpowiedzi | **Ty** |
| `GET,POST /Forum/CreateThread` | `Forum/CreateThread` | formularz tworzenia wątku | **Ty** |
| `GET,POST /Forum/EditMessage`, `POST DeleteMessage` | `Forum/EditMessage` | edycja/usuwanie (moderator/admin) | **Ty** |
| `GET,POST /Account/Login` | `Account/Login` | logowanie | znajomy |
| `GET,POST /Account/Register` | `Account/Register` | rejestracja | znajomy |
| `GET /Account/Profile` | `Account/Profile` | karta profilu + statystyki | znajomy |
| `GET,POST /Account/Settings` | `Account/Settings` | konto, hasło, paginacja, bezczynność, awatar, motyw | znajomy |
| `GET /Admin` | `Admin/Index` | dashboard: liczby + skróty + ogłoszenia | znajomy |
| `GET /Admin/Users` | `Admin/Users` | lista użytkowników, rola, ranga, ban | znajomy |
| `GET,POST /Admin/Categories` | `Admin/Categories` | CRUD kategorii | znajomy |
| `GET,POST /Admin/Forums` | `Admin/Forums` | CRUD forów: kategoria, uprawnienia anonimowe, moderatorzy | znajomy |
| `GET,POST /Admin/Announcements` | `Admin/Announcements` | CRUD ogłoszeń | znajomy |

Topbar i stopka są wspólne (`Views/Shared/_Topbar.cshtml`, `_Footer.cshtml`) — powielane automatycznie przez `_Layout`.

---

## 6. Walidacja formularzy

| Formularz | Walidacja klient (jQuery Unobtrusive + HTML5) | Walidacja serwer (ModelState) |
|---|---|---|
| Rejestracja | nazwa 3–30 znaków, e-mail, hasło min. 8 znaków z literą i cyfrą, zgodność haseł, regulamin | to samo na ViewModelu |
| Logowanie | pola wymagane, format e-maila | to samo |
| Nowy wątek | forum wybrane, tytuł 5–120, treść min. 10 znaków | to samo |
| Odpowiedź | treść min. 2 znaków | to samo |
| Kategorie / fora (admin) | nazwa wymagana, maks. długości | to samo |
| Awatar | typ (jpg/png/webp), maks. 512×512 px, maks. 200 kB | walidacja w JS przed zapisem |

---

## 7. Ryzyka

| Ryzyko | Wpływ | Mitygacja |
|---|---|---|
| Rozjazd wspólnych plików przy pracy równoległej | konflikty git | twardy podział plików (dok. 02), wspólne pliki tylko po uzgodnieniu |
| Brak weryfikacji uprawnień po stronie serwera w T1 | luka w T2 | jawny TODO w kontrolerach; T2: `[Authorize]` + sprawdzenie w repozytorium |
| Zakres 64 pkt przy limicie 45 | stratne godziny | selekcja z sekcji 3, warstwa B jako minimum |
| Skomplikowane funkcje (HTML sanitizer, i18n, poczta) | opóźnienie T3 | odłożone świadomie; wymagają decyzji prowadzącego |
| Praca na `localStorage` bez testów jednostkowych | regresje | trzymamy logikę w małych, czystych modułach JS (`auth.js`, `theme.js`) |

---

## 8. Kryteria odbioru etapu 1 (checklist dla prowadzącego)

- [ ] Aplikacja uruchamia się poleceniem `dotnet run` i pokazuje stronę główną z danymi demonstracyjnymi (bez ręcznego wpisywania danych).
- [ ] Topbar obecny na każdym widoku: powrót do domu, lista wątków, po prawej — menu użytkownika (profil, ustawienia, wyloguj) lub gość (zaloguj, zarejestruj), przełącznik 3 skórek, „Panel administracyjny" widoczny tylko dla admina.
- [ ] Rejestracja i logowanie działają z pełną walidacją po stronie klienta; po zalogowaniu nazwa użytkownika widoczna w topbarze i profilu.
- [ ] Lista wątków: paginacja, wyszukiwarka, przycisk tworzenia wątku, statystyki (odpowiedzi, odsłony).
- [ ] Strona wątku: wiadomość inicjująca, odpowiedzi, formularz odpowiedzi, licznik odsłon rośnie.
- [ ] Tworzenie wątku: formularz z walidacją, po zapisie nowy wątek widoczny na liście.
- [ ] Panel admina: użytkownicy, kategorie, fora, ogłoszenia — formularze CRUD działają na danych w pamięci.
- [ ] Trzy skórki przełączają się z zapamiętaniem wyboru.
- [ ] Kod źródłowy zawiera podział na warstwy MVC (Controllers / Models / Views / Services) zgodnie z dobrymi praktykami .NET.
