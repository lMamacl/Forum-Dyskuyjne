# Zadanie „Forum dyskusyjne” — analiza wymagań prostym językiem

**Przedmiot:** Aplikacje webowe w ASP MVC (ASP.NET Core MVC, .NET 10)
**Repozytorium:** https://github.com/lMamacl/Forum-Dyskuyjne
**Data analizy:** 15.11.2026 · **wersja dokumentu:** 2.0 (po doprecyzowaniu modelu forum/kategorie)

> **Po co ten dokument?** Tłumaczy oficjalne zasady zadania na codzienny język:
> co trzeba zrobić, jak nazywamy elementy ekranu i gdzie to widać w aplikacji.
> Założenia techniczne (architektura, trasy, walidacje) są na końcu — można je
> przeczytać później. Podział pracy: `docs/02-plan-pracy-i-wdrozenia.md`.

---

## 1. O co chodzi w zadaniu?

Masz zbudować **forum dyskusyjne** — serwis, na którym ludzie zakładają wątki
(i odpowiadają na nie), a administrator zarządza strukturą i porządkiem.

Do zdobycia jest maksymalnie **45 punktów**. W puli jest jednak **64 punkty**
— czyli nie wszystko naraz; trzeba **wybrać** funkcje i zrobić je porządnie.

Prace idą w **3 terminach**:

| Termin | Co dostarczamy | Cel |
|---|---|---|
| **T1** (pilne) | Formularze i widoki — wszystko klikalne, z walidacją, na danych demonstracyjnych | Kompletny interfejs do pokazania prowadzącemu |
| **T2** | Baza danych + prawdziwe konta i sesje | Dane przetrwają wyłączenie serwera |
| **T3** | Funkcje dodatkowe + dokumentacja (PDF/ODT/DOC) | Ostateczny odbiór |

---

## 2. Słowniczek — jak nazywamy rzeczy

Często używane nazwy w tym dokumencie i w aplikacji:

| Nazwa | Co to znaczy |
|---|---|
| **topbar** | Pasek na samym górze ekranu, widoczny na każdym widoku. Po lewej: logo + nazwa „Forum Dyskusyjne” (klik = powrót na stronę główną) i zakładka **Fora**; po prawej: menu użytkownika, przełącznik motywów, przycisk wylogowania |
| **stopka** | Pasek na samym dole ekranu (odnośniki: strona główna, kategorie i fora, wątki, logowanie…) |
| **zakładka „Fora”** | Główny widok forum — lista kategorii, a w nich fora. To **domyślny widok** po wejściu na forum |
| **forum** | Miejsce, w którym żyją wątki (np. „Programowanie”). Każdy wątek należy do **dokładnie jednego** forum |
| **kategoria (grupowanie for)** | Pojemnik na fora (np. „Technika” zawiera fora „Programowanie” i „Sprzęt i sieci”). **Każde forum należy do dokładnie jednej kategorii.** Kategorie i fora tworzy tylko administrator |
| **kategoria wątku = tag** | Etykieta opisująca treść wątku (np. „Początkujący”, „Hobby”). **Jeden wątek może mieć kilka tagów** (i musi mieć co najmniej jeden). Słownik tagów też tworzy administrator — z tego samego spisu kategorii. Służy do filtrowania |
| **wątek (thread)** | Pytanie lub temat wrzucony na forum: tytuł + krótki opis + treść, a pod nim odpowiedzi |
| **odpowiedź (wiadomość)** | Treść dopisywana pod wątkiem |
| **przyklejony** | Wątek przypięty na górze listy swojego forum — ustawia tylko administrator |
| **panel filtrów** | Rozwijany panel na liście wątków przyciskiem **Filtry** — wybierasz tam: forum, tag i sortowanie |
| **fraza / wyszukiwarka** | Pole tekstowe „Szukaj…” — szuka po tytule, opisie i treści wiadomości |
| **sortowanie** | Kolejność na liście: najaktywniejsze, najnowsze, najwięcej odpowiedzi, najwięcej odsłon |
| **stronicowanie (paginacja)** | Numerowane strony listy, gdy wątków jest dużo; ustawiane w profilu |
| **uprawnienia anonimowe** | Przyciski w formularzu admina: „niezalogowani mogą **czytać** to forum” i „…mogą **pisać**”. To za to są punkty |
| **moderator** | Użytkownik z prawem edytowania i usuwania wiadomości na **swoich** forach (może mieć kilka for) |
| **pulpit admina** | Strona `/Admin` z liczbami i skrótami do zarządzania |
| **ogłoszenie (announcement)** | Komunikat administratora na stronie głównej |
| **motyw (skórka)** | Wygląd kolorystyczny — trzy warianty do wyboru w topbarze |

### Najważniejsze: forum ≠ kategoria (to dwa różne pojęcia!)

```
KATEGORIA (grupowanie for)          KATEGORIA WĄTKU (= tag)
np. „Technika”                      np. „Początkujący”
   └── forum „Programowanie”           przypisywana wątkowi przy tworzeniu,
   └── forum „Sprzęt i sieci”          można nadać kilka, służy do filtrowania
```

- **Forum** decyduje: gdzie wątek mieszka, kto go może czytać/pisać, kto go moderuje.
- **Tag** decyduje: czego wątek dotyczy — dlatego da się go filtrować na liście.
- Jeden wątek → **jedno forum** + **kilka tagów**.

---

## 3. Punkty — za co dokładnie dostaje się punkty

Ocena trudności: **N** — niska, **Ś** — średnia, **W** — wysoka.

| Lp. | Funkcjonalność (po ludzku) | Pkt | Trudn. | Termin | Stan na dziś |
|---|---|---|---|---|---|
| 1 | Zakładanie wątków i dopisywanie odpowiedzi | 4 | N | T1+T2 | Gotowe (dane w pamięci) |
| 2 | Rejestracja, logowanie, profil, wylogowanie | 4 | Ś | T1+T2 | Formularze z walidacją; sesja po stronie przeglądarki |
| 3 | Panel administratora: wiadomości i użytkownicy | 4 | Ś | T1+T2 | Pulpit + lista użytkowników |
| 4 | Tylko admin tworzy kategorie i fora; forum należy do 1 kategorii | 4 | N | T1+T2 | Gotowe — formularze w panelu admina |
| 5 | Uprawnienia anonimowe per forum (czytanie / pisanie) | 2 | N | T2 | Pola w formularzu admina, model gotowy |
| 6 | Moderatorzy wielu for + edycja/usuwanie wiadomości | 4 | Ś | T1+T2 | Przyciski w widokach, model relacji |
| 7 | Statystyki: użytkownicy, wątki, wiadomości, odsłony, odpowiedzi | 2 | N | T1 | Gotowe — liczniki z danych demonstracyjnych |
| 8 | Wątki przyklejone (ustawia tylko admin) | 2 | N | T1+T2 | Wyświetlanie na górze listy + checkbox dla admina |
| 9 | Ogłoszenia administratora na stronie głównej | 2 | N | T1+T2 | Sekcja na stronie głównej + formularz w adminie |
| 10 | Wiadomości prywatne (odczyt, usuwanie) | 2 | Ś | T2/T3 | — |
| 11 | Wylogowanie po bezczynności | 2 | N | T1 | Licznik w `auth.js`, ustawienie w profilu |
| 12 | Awatary (limit rozdzielczości i wielkości w kB) | 2 | N | T1 | Gotowe — walidacja typu, 512×512 px, 200 kB |
| 13 | Stronicowanie list + ustawienie w profilu | 2 | N | T1 | Gotowe — numeracja stron, rozmiar z cookie |
| 14 | Wyszukiwanie słów kluczowych w wiadomościach | 2 | N | T1 | Gotowe — pole frazy na liście wątków |
| 15 | Operatory AND / OR / NOT w wyszukiwaniu | 2 | Ś | T2/T3 | — |
| 16 | Załączniki (limit liczby i wielkości) | 2 | Ś | T2/T3 | — |
| 17 | Zgłoszenie wiadomości do moderacji | 2 | N | T2/T3 | — |
| 18 | Słownik słów zakazanych (admin) | 4 | Ś | T3 | — |
| 19 | Emotikony w treści wiadomości | 2 | N | T2/T3 | — |
| 20 | HTML z listą dozwolonych znaczników (sanitizer) | 4 | W | T3 | — |
| 21 | Rangi automatyczne + ręczna zmiana przez admina | 2 | N | T2 | Kolumna rangi w panelu admina |
| 22 | Bezpieczna procedura przypominania i zmiany hasła | 2 | W | T2/T3 | Wymaga wysyłki e-mail |
| 23 | Trzy motywy wyglądu (2 istotne + trzeci) | 3 | N | **T1** | **Gotowe** — przełącznik w topbarze |
| 24 | Dodatkowe wersje językowe (2 + 1 pkt) | 3 | W | T3 | — |
| | **RAZEM w puli** | **64** | | | **cel: 45 pkt** |

---

## 4. Co warto wybrać do 45 punktów

### Warstwa A — rdzeń (28 pkt)

Bez tego forum nie istnieje: `1` (4) + `2` (4) + `3` (4) + `4` (4) + `5` (2) + `6` (4) + `7` (2) + `8` (2) + `9` (2) = **28 pkt**.

### Warstwa B — najlepszy stosunek punktów do pracy (17 pkt)

| Funkcja | Pkt | Dlaczego warto |
|---|---|---|
| `13` Stronicowanie + ustawienie w profilu | 2 | Paginacja i tak jest potrzebna |
| `14` Wyszukiwanie w forum | 2 | Formularz GET + jedno zapytanie |
| `11` Wylogowanie po bezczynności | 2 | Ok. 20 linii JavaScriptu |
| `19` Emotikony | 2 | Zamiana `:)` na obrazki przy renderowaniu |
| `21` Rangi | 2 | Progi w repozytorium + dropdown w adminie |
| `15` Operatory AND/OR/NOT | 2 | Naturalne rozszerzenie wyszukiwania |
| `17` Zgłoszenia do moderacji | 2 | Formularz + kolejka w panelu admina |
| `23` Motywy (3 warianty) | 3 | **Już zrobione** |
| | **17 pkt** | |

**Razem A + B = 45 pkt.**

### Warstwa C — rezerwa (0–19 pkt zależnie od czasu)

Priorytet: `12` awatary (2 — **zrobione**), `10` wiadomości prywatne (2),
`18` słownik słów zakazanych (4), `16` załączniki (2), `22` reset hasła (2),
`20` HTML ze znacznikami (4), `24` języki (3).

**Strategia:** planować 48–50 pkt, żeby odjąć ewentualne punkty za niekompletne
elementy i mimo to zmieścić się na 45.

---

## 5. Jak to działa w środku (dla porządku)

```
Przeglądarka — formularze z walidacją po stronie klienta
      │
      ▼
Kontrolery (Home, Forum, Account, Admin) — walidacja, przekierowania
      │
      ▼
Repozytorium (jeden interfejs IForumRepository)
      ├── teraz: dane w pamięci programu + dane demonstracyjne (T1)
      └── w T2: podmiana na bazę danych — widoki i kontrolery bez zmian
```

**Zasady:** każdy widok ma kontroler i swój model widoku (ViewModel);
formularze korzystają z tag helperów ASP.NET; cała treść (kategorie, fora,
wątki, wiadomości, ogłoszenia, statystyki) przechodzi przez **jedno
repozytorium** — dzięki temu w T2 zamieniamy tylko jego implementację.
Konta w T1 żyją po stronie przeglądarki (`localStorage`), w T2 wchodzi
ASP.NET Identity — bez przepisywania widoków.

### Główne obiekty danych (T1 — bez bazy)

| Obiekt | Co przechowuje | Ważna reguła |
|---|---|---|
| **Kategoria** | nazwa, opis, kolejność | zawiera wiele forów |
| **Forum** | nazwa, opis, kategoria, uprawnienia anonimowe, moderatorzy | **należy do dokładnie jednej kategorii** |
| **Wątek** | forum (jedno!), kilka tagów, tytuł, opis, treść, autor, flaga „przyklejony”, licznik odsłon | **jeden wątek → jedno forum + kilka tagów** |
| **Wiadomość** | wątek, autor, treść, data, data edycji | edytuje autor, moderator lub admin |
| **Użytkownik** | nazwa, e-mail, rola, ranga, licznik wiadomości, aktywność | lista w panelu admina |
| **Ogłoszenie** | tytuł, treść, autor, data | strona główna |

### Co się zmieni w T2/T3 (a co zostaje)

| Element | Teraz (T1) | Potem | Zmiana widoków |
|---|---|---|---|
| Treść forum | dane w pamięci + demonstracyjne | baza danych | brak |
| Konta i sesja | `localStorage` | ASP.NET Identity | brak |
| Wyszukiwanie | proste „wszystkie słowa” | operatory AND/OR/NOT | brak |
| Emotikony / HTML | tekst surowy | filtr + sanitizer | minimalne |

### Decyzje projektowe (dlaczego tak, a nie inaczej)

1. **Odpowiedzi dopisuje się na stronie wątku**, osobny widok tylko do
   zakładania wątku — mniej tras, naturalny UX forów.
2. **Stronicowanie po stronie serwera** (`?page=`) — standardowy, łatwy do
   wytłumaczenia wzorzec.
3. **Wyszukiwarka to zwykły formularz GET na liście wątków** — fraza leci
   przez kontroler do repozytorium; operatory AND/OR/NOT w T2 to parser tego
   pola.
4. **Trzy motywy na zmiennych CSS** + atrybut na `<html>`; wybór zapisywany
   w przeglądarce i odczytywany przed pierwszym rysunkiem (bez migotania).
5. **Statystyki liczy repozytorium, nie widok** — w T2 to samo zapytanie do bazy.
6. **Licznik odsłon rośnie** przy wejściu na wątek — w T2 jeden zapis do bazy.
7. **Forum wątku wybiera się przy tworzeniu** (jedno na wątek) — to forum
   decyduje o uprawnieniach i moderacji. **Tagów może być kilka** — służą
   wyłącznie do opisu i filtrowania, nie mylić z kategorią for.

---

## 6. Ekrany i trasy (co gdzie kliknąć)

| Adres | Ekran | Opis | Kto robi (T1) |
|---|---|---|---|
| `/` | Strona główna | powitanie, statystyki, ogłoszenia, ostatnie wątki | wspólny |
| `/Forum` | **Zakładka „Fora” (widok domyślny)** | kategorie → fora; na górze pasek: wyszukiwarka wszystkich wątków, „Wszystkie wątki”, „Nowy wątek” | **Ty** |
| `/Forum/Threads` (+ `?q=&forumId=&categoryId=&sort=&page=`) | Lista wątków | globalna albo z filtrem forum; panel filtrów: fraza, **forum**, **tag**, sortowanie; paginacja; „Nowy wątek” | **Ty** |
| `/Forum/Thread?id=` | Strona wątku | treść, tagi, odpowiedzi, formularz odpowiedzi, licznik odsłon | **Ty** |
| `/Forum/CreateThread` | Nowy wątek | **wybór forum** + **kategorie-tagi (wiele, min. 1)**, tytuł, krótki opis, treść | **Ty** |
| `/Forum/EditMessage` | Edycja wiadomości | edycja/usuwanie (autor, moderator, admin) | **Ty** |
| `/Account/Login`, `/Account/Register` | Logowanie / rejestracja | formularze z walidacją | znajomy |
| `/Account/Profile` | Profil | awatar, ranga, statystyki, ostatnie wypowiedzi | znajomy |
| `/Account/Settings` | Ustawienia | konto, hasło, stronicowanie, bezczynność, awatar, motyw | znajomy |
| `/Admin` | Pulpit admina | liczby + skróty | znajomy |
| `/Admin/Users` | Użytkownicy | rola, ranga, ban | znajomy |
| `/Admin/Categories` | Kategorie | dodawanie/edycja/usuwanie — słownik for **i tagów** | znajomy |
| `/Admin/Forums` | Fora | nazwa, kategoria, uprawnienia anonimowe, moderatorzy | znajomy |
| `/Admin/Announcements` | Ogłoszenia | dodawanie/edycja/usuwanie | znajomy |

**Topbar** i **stopka** są wspólne dla wszystkich widoków (osobne pliki,
wciągane automatycznie przez główny układ strony).

---

## 7. Walidacja formularzy (komunikaty po polsku)

| Formularz | Po stronie przeglądarki | Po stronie serwera |
|---|---|---|
| Rejestracja | nazwa 3–30 znaków, e-mail, hasło min. 8 znaków z literą i cyfrą, zgodność haseł, regulamin | to samo |
| Logowanie | pola wymagane, format e-maila | to samo |
| Nowy wątek | forum wybrane, **min. 1 tag**, tytuł 5–120, treść min. 10 znaków | to samo |
| Odpowiedź | treść min. 2 znaków | to samo |
| Kategorie / fora (admin) | nazwa wymagana, maks. długości | to samo |
| Awatar | typ (jpg/png/webp), maks. 512×512 px, maks. 200 kB | to samo (w przeglądarce) |

---

## 8. Ryzyka

| Ryzyko | Wpływ | Co robimy |
|---|---|---|
| Rozjazd wspólnych plików przy pracy we dwóch | konflikty w git | twardy podział plików (dok. 02); wspólne pliki tylko po uzgodnieniu |
| Brak sprawdzania uprawnień na serwerze w T1 | luka w T2 | jawne komentarze TODO; w T2 atrybuty dostępu |
| 64 pkt przy limicie 45 | stratne godziny | wybór z sekcji 4; warstwa A to minimum |
| Trudne funkcje (sanitizer HTML, języki, poczta) | opóźnienie T3 | odłożone świadomie, do ustalenia z prowadzącym |
| Brak testów jednostkowych w T1 | regresje | mała, czysta logika w osobnych plikach JavaScriptu |

---

## 9. Kryteria odbioru etapu 1 (checklista dla prowadzącego)

- [ ] Aplikacja uruchamia się poleceniem `dotnet run` i od razu pokazuje
      stronę główną z danymi demonstracyjnymi.
- [ ] **Topbar** na każdym widoku: logo/nazwa forum (powrót do domu), zakładka
      „Fora”, po prawej — menu użytkownika (profil, ustawienia, wyloguj) albo
      gość (zaloguj, zarejestruj), przełącznik 3 motywów, „Panel
      administracyjny” tylko dla admina.
- [ ] Rejestracja i logowanie z pełną walidacją; po zalogowaniu nazwa
      użytkownika widoczna w topbarze.
- [ ] **Domyślny widok** (`/Forum`): przegląd kategorii i forów; kliknięcie
      forum otwiera listę jego wątków.
- [ ] **Lista wątków**: panel filtrów (fraza, forum, tag, sortowanie),
      stronicowanie, statystyki przy wierszach, przycisk „Nowy wątek”.
- [ ] **Strona wątku**: treść, tagi, odpowiedzi, formularz odpowiedzi,
      licznik odsłon rośnie.
- [ ] **Tworzenie wątku**: wybór forum (jedno) + tagi (wiele, min. 1),
      walidacja tytułu i treści; po zapisie wątek widoczny na liście.
- [ ] **Panel admina**: użytkownicy, kategorie (for i tagów), fora
      (kategoria, uprawnienia anonimowe, moderatorzy), ogłoszenia — formularze
      działają na danych w pamięci; nie da się usunąć kategorii z forami lub wątkami.
- [ ] Trzy motywy przełączają się i wybór zapamiętuje się po odświeżeniu.
- [ ] Kod podzielony na warstwy MVC (kontrolery / modele / widoki / serwisy)
      zgodnie z dobrymi praktykami .NET.

---

## 10. Mapa plików (dla programisty)

Tu już wyłącznie techniczne identyfikatory — dla osoby piszącej kod:

| Element w aplikacji | Plik |
|---|---|
| topbar | `Views/Shared/_Topbar.cshtml` |
| stopka | `Views/Shared/_Footer.cshtml` |
| główny układ (szkielet strony) | `Views/Shared/_Layout.cshtml` |
| strona główna | `Views/Home/Index.cshtml` |
| zakładka „Fora” (przegląd) | `Views/Forum/Browse.cshtml` |
| lista wątków | `Views/Forum/Threads.cshtml` |
| strona wątku | `Views/Forum/Thread.cshtml` |
| nowy wątek | `Views/Forum/CreateThread.cshtml` |
| edycja wiadomości | `Views/Forum/EditMessage.cshtml` |
| konta | `Views/Account/*.cshtml`, kontroler `AccountController` |
| panel admina | `Views/Admin/*.cshtml`, kontroler `AdminController` |
| logika forum (dane) | `Services/IForumRepository.cs`, `Services/InMemoryForumRepository.cs` (T1), `Services/EfForumRepository.cs` (T2) |
| dane demonstracyjne | `Services/SeedData.cs` |
| sesja w T1 (konto w przeglądarce) | `wwwroot/js/auth.js` |
| motywy | `wwwroot/js/theme.js`, `wwwroot/css/site.css` |
| walidacja po stronie klienta | `Views/Shared/_ValidationScriptsPartial.cshtml` |
