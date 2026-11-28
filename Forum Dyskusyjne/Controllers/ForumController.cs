using Forum_Dyskusyjne.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Forum_Dyskusyjne.Controllers;

public class ForumController : Controller
{
    private readonly ForumDbContext _context;

    // Kontener ASP.NET automatycznie dostarcza zarejestrowany w Program.cs ForumDbContext
    public ForumController(ForumDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // AKCJA 1: Index()
    // URL:     GET /Forum   lub   GET /Forum/Index
    // WIDOK:   Views/Forum/Index.cshtml
    // ZADANIE: Pobrać listę wszystkich działów forum i pokazać ile każdy ma wątków.
    public async Task<IActionResult> Index()
    {
        // BAZA: Pytamy tabelę dbo.Forums o wszystkie wiersze.
        // .Include(f => f.Threads) robi SQL-owy LEFT JOIN z dbo.Threads,
        // dzięki czemu każde forum w C# ma od razu wczytaną listę swoich wątków.
        var forums = await _context.Forums
            .Include(f => f.Threads)
            .OrderBy(f => f.Id)
            .ToListAsync();

        // WIDOK: Przekazujemy listę obiektów Forum jako model do widoku Index.cshtml
        return View(forums);
    }

    // ================================================================
    // AKCJA 2: Threads(int forumId)
    // URL:     GET /Forum/Threads?forumId=1
    // WIDOK:   Views/Forum/Threads.cshtml
    // ZADANIE: Pobrać wybrane forum wraz z jego wątkami i liczbą odpowiedzi w każdym wątku.
    public async Task<IActionResult> Threads(int forumId)
    {
        // BAZA: Szukamy jednego forum o zadanym ID (WHERE Id = forumId).
        // .Include(f => f.Threads)         -> dociąga wątki z dbo.Threads dla tego forum
        // .ThenInclude(t => t.Posts)       -> dociąga odpowiedzi z dbo.Posts dla każdego wątku
        var forum = await _context.Forums
            .Include(f => f.Threads)
                .ThenInclude(t => t.Posts)
            .FirstOrDefaultAsync(f => f.Id == forumId);

        // KONTROLA BŁĘDÓW: Jeśli użytkownik wpisze w URL np. ?forumId=9999
        if (forum == null)
        {
            return NotFound("Nie znaleziono takiego forum.");
        }

        // WIDOK: Przekazujemy jedno konkretne Forum (ze wszystkimi wątkami w środku)
        return View(forum);
    }

    // ================================================================
    // AKCJA 3: Thread(int id)
    // URL:     GET /Forum/Thread?id=1
    // WIDOK:   Views/Forum/Thread.cshtml
    // ZADANIE: Pobrać wątek główny, nazwę działu nadrzędnego oraz listę odpowiedzi.
    public async Task<IActionResult> Thread(int id)
    {
        // BAZA: Szukamy wątku w dbo.Threads po jego Id.
        // .Include(t => t.Forum) -> potrzebne do breadcrumbu (żeby wiedzieć do jakiego forum wrócić)
        // .Include(t => t.Posts) -> dociągamy wszystkie odpowiedzi powiązane z tym wątkiem
        var thread = await _context.Threads
            .Include(t => t.Forum)
            .Include(t => t.Posts)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (thread == null)
        {
            return NotFound("Nie znaleziono wątku.");
        }

        // WIDOK: Przekazujemy obiekt ForumThread
        return View(thread);
    }

    // ================================================================
    // AKCJA 4a: GET /Forum/CreateThread?forumId=1
    // Wyświetla pusty formularz tworzenia wątku w konkretnym dziale 
    [Authorize] 
    [HttpGet]
    public async Task<IActionResult> CreateThread(int forumId)
    {
        var forum = await _context.Forums.FindAsync(forumId);
        if (forum == null)
        {
            return NotFound("Forum nie istnieje.");
        }
        var model = new ViewModels.CreateThreadViewModel
        {
            ForumId = forum.Id,
            ForumName = forum.Name
        };
        return View(model);
    }

    // ================================================================
    // AKCJA 4b: POST /Forum/CreateThread
    // Odbiera dane z formularza, waliduje je i zapisuje nowy wiersz w dbo.Threads
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateThread(ViewModels.CreateThreadViewModel model)
    {
        // 1. Walidacja serwerowa
        if (!ModelState.IsValid)
        {
            // Jeśli dane są błędne, ponownie dociągamy nazwę forum i zwracamy widok z błędami
            var forum = await _context.Forums.FindAsync(model.ForumId);
            model.ForumName = forum?.Name;
            return View(model);
        }
        var forumExists = await _context.Forums.AnyAsync(f => f.Id == model.ForumId);
        if (!forumExists)
        {
            // Jeśli forum nie istnieje, zwracamy błąd (np. NotFound lub dedykowany komunikat)
            return NotFound("Podane forum nie istnieje.");
        }
        // 2. Mapowanie ViewModelu na encję bazodanową
        // (Na Termin 1 ustawiamy tymczasowego autora, w Terminie 2 pobierzemy go z sesji usera)
        var thread = new Models.ForumThread
        {
            ForumId = model.ForumId,
            Title = model.Title,
            Content = model.Content,
            AuthorUsername = User.Identity?.IsAuthenticated == true ? User.Identity.Name! : "JanKowalski",
            CreatedAt = DateTime.UtcNow
        };
        // 3. Zapis do bazy danych przez EF Core
        _context.Threads.Add(thread);
        await _context.SaveChangesAsync();
        // 4. Wzorzec PRG (Post-Redirect-Get) -> przekierowujemy do nowo utworzonego wątku
        return RedirectToAction(nameof(Thread), new { id = thread.Id });
    }

    // ================================================================
    // AKCJA 5: POST /Forum/AddPost
    // Dodaje nową odpowiedź do istniejącego wątku (zapis w dbo.Posts)
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPost(ViewModels.CreatePostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["PostError"] = "Odpowiedź musi mieć od 3 do 2000 znaków.";
            return RedirectToAction(nameof(Thread), new { id = model.ThreadId });
        }
        var post = new Models.ForumPost
        {
            ThreadId = model.ThreadId,
            Content = model.Content,
            AuthorUsername = User.Identity?.Name ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Thread), new { id = model.ThreadId });
    }

    // ================================================================
}