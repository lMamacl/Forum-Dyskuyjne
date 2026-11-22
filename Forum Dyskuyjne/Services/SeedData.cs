using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Services;

/// <summary>
/// Dane demonstracyjne ładowane przy starcie aplikacji — dzięki nim wszystkie funkcjonalności
/// można sprawdzić bez ręcznego wpisywania czegokolwiek do bazy (wymaganie z zasad zaliczenia).
/// W etapie 2 zastąpione migracjami i seederem bazy danych.
/// </summary>
public static class SeedData
{
    public sealed class Snapshot
    {
        public List<Category> Categories { get; } = new();
        public List<Forum> Forums { get; } = new();
        public List<ForumThread> Threads { get; } = new();
        public List<ForumMessage> Messages { get; } = new();
        public List<UserAccount> Users { get; } = new();
        public List<Announcement> Announcements { get; } = new();
    }

    public static Snapshot Build()
    {
        var s = new Snapshot();
        var now = DateTime.UtcNow;

        // ---- Kategorie ----------------------------------------------------
        s.Categories.Add(new Category { Id = 1, Name = "Ogólne", Description = "Rozmowy na każdy temat i powitania nowych użytkowników.", Position = 0 });
        s.Categories.Add(new Category { Id = 2, Name = "Technika", Description = "Programowanie, sprzęt i sieci.", Position = 1 });
        s.Categories.Add(new Category { Id = 3, Name = "Hobby", Description = "Gry, muzyka i film.", Position = 2 });

        // ---- Fora (każde forum należy do dokładnie jednej kategorii) -------
        s.Forums.Add(new Forum { Id = 1, CategoryId = 1, Position = 0, Name = "Rozmowa ogólna", Description = "Swobodne rozmowy o wszystkim.", AllowAnonymousView = true, AllowAnonymousPost = false });
        s.Forums.Add(new Forum { Id = 2, CategoryId = 1, Position = 1, Name = "Powitalnia", Description = "Przedstaw się reszcie społeczności.", AllowAnonymousView = true, AllowAnonymousPost = true });
        s.Forums.Add(new Forum { Id = 3, CategoryId = 2, Position = 0, Name = "Programowanie", Description = "Języki, frameworki i dobre praktyki.", AllowAnonymousView = true, AllowAnonymousPost = false, ModeratorIds = new List<int> { 2, 4 } });
        s.Forums.Add(new Forum { Id = 4, CategoryId = 2, Position = 1, Name = "Sprzęt i sieci", Description = "Konfiguracje, awarie, zakupy.", AllowAnonymousView = true, AllowAnonymousPost = false, ModeratorIds = new List<int> { 2 } });
        s.Forums.Add(new Forum { Id = 5, CategoryId = 3, Position = 0, Name = "Gry i rozrywka", Description = "Recenzje, porady i wspólne granie.", AllowAnonymousView = true, AllowAnonymousPost = false, ModeratorIds = new List<int> { 5 } });
        s.Forums.Add(new Forum { Id = 6, CategoryId = 3, Position = 1, Name = "Muzyka i film", Description = "Dla fanów dobrych brzmień i obrazu.", AllowAnonymousView = false, AllowAnonymousPost = false, ModeratorIds = new List<int> { 5 } });

        // ---- Użytkownicy (odpowiedniki kont demonstracyjnych auth.js) ------
        s.Users.Add(User(1, "admin", "admin@forum.pl", UserRole.Admin, "Administrator", 42, now.AddMonths(-14)));
        s.Users.Add(User(2, "moderator", "mod@forum.pl", UserRole.User, "Moderator", 128, now.AddMonths(-12)));
        s.Users.Add(User(3, "user", "user@forum.pl", UserRole.User, "Uczestnik", 17, now.AddMonths(-6)));
        s.Users.Add(User(4, "kowalski", "kowalski@forum.pl", UserRole.User, "Weteran", 96, now.AddMonths(-10)));
        s.Users.Add(User(5, "anna.nowak", "anna@forum.pl", UserRole.User, "Weteran", 74, now.AddMonths(-9)));
        s.Users.Add(User(6, "programista87", "p87@forum.pl", UserRole.User, "Uczestnik", 23, now.AddMonths(-4)));
        s.Users.Add(User(7, "zet_emeryt", "zet@forum.pl", UserRole.User, "Nowicjusz", 3, now.AddDays(-20)));

        // ---- Wątki i wiadomości ------------------------------------------
        // Forum 3 — Programowanie (12 wątków: demonstracja stronicowania)
        int t = 1;
        AddThread(s, t++, 3, "Jak zacząć przygodę z C# i ASP.NET Core?", "Ścieżka nauki dla początkujących — materiały, ćwiczenia, projekty.",
            "Cześć, chcę zacząć uczyć się C# z myślą o tworzeniu aplikacji webowych. Od czego polecacie zacząć — samego syntaxu, czy od razu od projektu? Interesuje mnie zwłaszcza architektura MVC i to, jak wygląda praca z bazami danych.", "admin", true, now.AddDays(-30));
        AddThread(s, t++, 3, "Wzorzec MVC — dobre praktyki", "Gdzie trzymać logikę biznesową i jak dzielić odpowiedzialność?",
            "Przeglądam różne projekty i widzę, że logika biznesowa często trafia do kontrolerów. Jak Wy to organizujecie — serwisy, repozytoria, a może coś prostszego przy mniejszych projektach?", "admin", true, now.AddDays(-25));
        AddThread(s, t++, 3, "Entity Framework Core vs Dapper — co wybrać?", "Porównanie na przykładzie typowego projektu studenckiego.",
            "EF Core daje szybki start, Dapper daje kontrolę. Którą opcję wybieracie do projektów, w których liczy się czas wdrożenia?", "kowalski", false, now.AddDays(-20));
        AddThread(s, t++, 3, "Dependency injection w praktyce", "Rejestracja usług i zakresy życia obiektów.",
            "W jakich sytuacjach rejestrujecie usługi jako Singleton, a kiedy jako Scoped? Często mylę się przy żywotności obiektów trzymających stan.", "programista87", false, now.AddDays(-18));
        AddThread(s, t++, 3, "Jak poprawnie obsługiwać formularze w Razor?", "Tag helpery, walidacja po stronie klienta i serwera.",
            "Chciałbym uprościć obsługę formularzy — obecnie mam dużo powtarzanego kodu w widokach. Jak to wygląda u Was?", "anna.nowak", false, now.AddDays(-15));
        AddThread(s, t++, 3, "Czy warto uczyć się .NET 10?", "Nowości i stabilność najnowszej wersji.",
            "Czy aktualna wersja frameworka jest już na tyle dojrzała, że warto na niej budować projekty, czy lepiej zostać przy poprzedniej?", "user", false, now.AddDays(-12));
        AddThread(s, t++, 3, "Typy referencyjne a wartościowe", "Kiedy co stosować — prosty przewodnik.",
            "Szukam intuicyjnego wytłumaczenia różnicy między typami referencyjnymi a wartościowymi, najlepieze na przykładach z codziennej pracy.", "zet_emeryt", false, now.AddDays(-9));
        AddThread(s, t++, 3, "async/await — najczęstsze pułapki", "Bloki deadlock, przepełnienie stosu, obsługa wyjątków.",
            "Zebraliśmy najczęstsze błędy przy programowaniu asynchronicznym — zapraszam do dyskusji i uzupełniania listy.", "kowalski", false, now.AddDays(-7));
        AddThread(s, t++, 3, "Testy jednostkowe w projekcie studenckim", "Czy warto pisać testy, gdy termin goni?",
            "Z jednej strony testy oszczędzają czas przy zmianach, z drugiej zabierają go na starcie. Jakie jest Wasze podejście?", "programista87", false, now.AddDays(-5));
        AddThread(s, t++, 3, "REST czy GraphQL w małej aplikacji?", "Porównanie podejść do API.",
            "Buduję małe API i zastanawiam się, czy nie przesadzam z REST-em, gdy potrzebuję elastycznych zapytań.", "anna.nowak", false, now.AddDays(-4));
        AddThread(s, t++, 3, "Zarządzanie paczkami NuGet", "Wersjonowanie i aktualizacje zależności.",
            "Jak organizujecie aktualizacje paczek w projektach, które są już na produkcji?", "user", false, now.AddDays(-2));
        AddThread(s, t++, 3, "Publikacja aplikacji na serwerze", "Kroki od builda do działającego adresu URL.",
            "Opis krok po kroku: publikacja aplikacji ASP.NET Core na przykładowym serwerze, najczęstsze problemy i jak je ominąć.", "moderator", false, now.AddHours(-30));

        // Pozostałe fora
        AddThread(s, t++, 1, "Zasady i moderacja — przeczytaj przed pisaniem", "Najważniejsze informacje o działaniu forum.", "Porządkujemy dyskusje: korzystaj z wyszukiwarki, pisz rzeczowo i zgłaszaj naruszenia. Pełny regulamin w ogłoszeniu na stronie głównej.", "admin", false, now.AddDays(-40));
        AddThread(s, t++, 1, "Wasze ulubione narzędzia deweloperskie", "Edytory, terminale, skróty.", "Każdy ma swój zestaw narzędzi bez których nie wyobraża sobie pracy. Jak wygląda Wasz?", "kowalski", false, now.AddDays(-6));
        AddThread(s, t++, 1, "Spotkanie użytkowników forum", "Propozycje terminu i miejsca.", "Rozkręcamy temat wspólnego spotkania online. Głosujcie w odpowiedziach na dogodny termin.", "moderator", false, now.AddDays(-3));
        AddThread(s, t++, 2, "Cześć, jestem nowy na forum", "Krótka prezentacja.", "Nazywam się Tomek, studiuję informatykę i interesuję się backendem. Pozdrawiam wszystkich!", "user", false, now.AddDays(-8));
        AddThread(s, t++, 4, "Komputer nie chce wstać po wymianie zasilacza", "Diagnoza problemu z rozruchem.", "Po wymianie zasilacza komputer kręci wentylatorami, ale nie dochodzi do ekranu BIOS. Co sprawdzić najpierw?", "zet_emeryt", false, now.AddDays(-10));
        AddThread(s, t++, 4, "Sieć Wi-Fi zrywa przy wielu urządzeniach", "Router, kanały, ustawienia.", "Przy trzech urządzeniach jednocześnie internet zaczyna przerywać. Czy to kanał, czy router już kończy żywot?", "programista87", false, now.AddDays(-5));
        AddThread(s, t++, 4, "Jaki dysk do serwera domowego?", "SSD vs HDD, pojemność, niezawodność.", "Buduję małe NAS i zastanawiam się nad kompromisem między pojemnością a szybkością.", "anna.nowak", false, now.AddDays(-2));
        AddThread(s, t++, 4, "Konfiguracja VPN na routerze", "Krok po kroku z użyciem WireGuard.", "Krótki poradnik konfiguracji WireGuard na popularnym routerze domowym — krok po kroku ze zrzutami.", "moderator", false, now.AddDays(-1));
        AddThread(s, t++, 5, "Gry, w które warto zagrać w ten weekend", "Pozycje na krótkie sesje.", "Szukam gier na 2–3 godzinne sesje, najlepiej dla dwóch osób. Znasz coś wartego polecenia?", "user", false, now.AddDays(-4));
        AddThread(s, t++, 5, "Retro-gaming — powrót do 16 bitów", "Klasyki i emulatory.", "Emulatory mają się świetnie — jaki tytuł z tamtej epoki warto odkryć na nowo?", "kowalski", false, now.AddDays(-2));
        AddThread(s, t++, 5, "Wieczorny quiz filmowy", "Zagadki dla kinomanów.", "Co tydzień nowa porcja zagadek. Odpowiadajcie w wiadomościach, najlepsi dostaną rangę.", "anna.nowak", false, now.AddHours(-50));
        AddThread(s, t++, 6, "Najlepsze soundtracki roku", "Głosowanie na albumy.", "Podawajcie albumy z muzyki filmowej, które zrobiły na Was największe wrażenie w tym roku.", "moderator", false, now.AddDays(-3));
        AddThread(s, t++, 6, "Seriale, które warto nadrobić", "Krótkie recenzje bez spoilerów.", "Lista seriali warte Waszego czasu — piszcie krótkie recenzje bez zdradzania zakończenia.", "anna.nowak", false, now.AddDays(-1));

        // Odpowiedzi (kilka wątków z żywą dyskusją)
        Replies(s, 1, now, ("kowalski", "Ja zaczynałem od samego syntaxu, ale najszybciej poszło, gdy od razu zrobiłem mały projekt — typowa lista zadań z bazą danych."), ("anna.nowak", "Polecam oficjalny tutorial ASP.NET Core, a potem własne rozwinięcie pomysłu. Teoria sama nie zostaje w głowie."), ("programista87", "Do tego dokumentacja Microsoft Learn — jest naprawdę dobrze napisana i po polsku da się ogarnąć."), ("moderator", "Zapisz też wątek, bo wracają do niego co chwila nowe osoby."), ("admin", "Dodaję na górę listy, bo to częste pytanie."));
        Replies(s, 2, now, ("programista87", "Logika biznesowa w serwisach, kontroler tylko koordynuje żądanie i zwraca widok. Przy małych projektach wystarczy warstwa serwisu nad repozytorium."), ("kowalski", "Zgadza się — kluczowe, żeby kontroler nie znał szczegółów zapytań do bazy."), ("anna.nowak", "U nas w projekcie pomogło też trzymanie walidacji w ViewModelach zamiast w kodzie akcji."), ("user", "Dzięki, od razu łatwiej poukładać pliki."));
        Replies(s, 3, now, ("moderator", "Do projektu studenckiego spokojnie wystarczy EF Core — migracje i tak samo idą w parze z nauką ORM."), ("kowalski", "Dokładnie, a Dapper dokładam dopiero tam, gdzie czuję, że EF generuje niepotrzebne zapytania."), ("zet_emeryt", "A przy nauce warto poznać oba i porównać, jak wygląda ten sam ekran w obu podejściach."));
        Replies(s, 4, now, ("anna.nowak", "Scope dla żądań HTTP, Singleton dla stanu bezpiecznego w wielu wątkach, Transient dla lekkich, bezstanowych pomocników."), ("programista87", "I uważaj na Singletony wstrzykujące usługi Scoped — to częste źródło błędów."));
        Replies(s, 5, now, ("user", "Tag helpery plus jquery-validation-unobtrusive — walidacja działa po obu stronach bez pisania własnego kodu."));
        Replies(s, 8, now, ("kowalski", "Nie mieszaj sync i async na tych samych ścieżkach — to najczęstsza przyczyna blokad."), ("programista87", "Do tego konfiguruj exception handling globalnie, a nie w każdym miejscu osobno."));
        Replies(s, 12, now, ("moderator", "Publikacja w skrócie: publish do folderu, przeniesienie na serwer, uruchomienie przez Kestrol lub IIS. Opiszę dokładniej w osobnym wątku."));
        Replies(s, 13, now, ("kowalski", "Bez sensownego opisu i tagów takie wpisy trudno potem odnaleźć — polecam od razu uzupełnić."));
        Replies(s, 17, now, ("anna.nowak", "Sprawdź, czy karta sieciowa nie negocjuje połączenia na 2,4 GHz — po zmianie kanału od razu było lepiej."));

        // Demonstracja wielu tagów na jednym wątku (kategorie = tagi).
        s.Threads.First(t => t.Id == 1).CategoryIds.Add(1);  // „Jak zacząć z C#”: Technika + Ogólne
        s.Threads.First(t => t.Id == 8).CategoryIds.Add(1);  // „Co dziś gracie?”: Hobby + Ogólne

        // ---- Ogłoszenia ---------------------------------------------------
        s.Announcements.Add(new Announcement
        {
            Id = 1,
            Title = "Witamy na forum",
            Content = "Cieszymy się, że tu jesteś. Przed pierwszym postem zapoznaj się z zasadami: pisz rzeczowo, korzystaj z wyszukiwarki i nie publikuj cudzych treści bez podania źródła.",
            AuthorName = "admin",
            CreatedAt = now.AddDays(-45)
        });
        s.Announcements.Add(new Announcement
        {
            Id = 2,
            Title = "Trwają prace nad nową odsłoną forum",
            Content = "Wdrażamy nowy wygląd i dodatkowe funkcje. Jeśli zauważysz błąd, zgłoś go w wątku „Zasady i moderacja”.",
            AuthorName = "admin",
            CreatedAt = now.AddDays(-7)
        });

        return s;
    }

    private static UserAccount User(int id, string name, string email, UserRole role, string rank, int messages, DateTime joined) =>
        new() { Id = id, Username = name, Email = email, Role = role, Rank = rank, MessageCount = messages, JoinedAt = joined, IsActive = true };

    private static void AddThread(Snapshot s, int id, int forumId, string title, string description, string content,
        string author, bool pinned, DateTime created)
    {
        s.Threads.Add(new ForumThread
        {
            Id = id,
            ForumId = forumId,
            Title = title,
            Description = description,
            Content = content,
            AuthorName = author,
            CategoryIds = new List<int> { CategoryForForum(forumId) },
            IsPinned = pinned,
            CreatedAt = created,
            LastActivityAt = created
        });
    }

    /// <summary>Podstawowy tag wątku w danych demo (zależny od forum); część wątków dostaje drugi tag niżej.</summary>
    private static int CategoryForForum(int forumId) => forumId switch
    {
        1 or 2 => 1,   // Ogólne
        3 or 4 => 2,   // Technika
        5 or 6 => 3,   // Hobby
        _ => 1
    };

    private static void Replies(Snapshot s, int threadId, DateTime now, params (string Author, string Content)[] replies)
    {
        var thread = s.Threads.First(x => x.Id == threadId);
        var at = thread.CreatedAt.AddHours(3);
        foreach (var (author, content) in replies)
        {
            s.Messages.Add(new ForumMessage
            {
                Id = s.Messages.Count + 1,
                ThreadId = threadId,
                AuthorName = author,
                Content = content,
                CreatedAt = at
            });
            at = at.AddHours(Random.Shared.Next(1, 20));
            thread.LastActivityAt = at;
        }
    }
}
