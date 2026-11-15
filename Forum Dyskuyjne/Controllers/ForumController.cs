using Forum_Dyskuyjne.Models.Domain;
using Forum_Dyskuyjne.Models.ViewModels;
using Forum_Dyskuyjne.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Forum_Dyskuyjne.Controllers;

/// <summary>
/// Kontroler treści forum: przegląd kategorii, lista wątków (paginacja, wyszukiwanie),
/// strona wątku, tworzenie wątku, edycja i usuwanie wiadomości.
///
/// Uwagi bezpieczeństwa (do zamknięcia w etapie 2 — patrz docs/02):
///  * w etapie 1 nie ma sesji serwerowej, więc akcje zapisu nie weryfikują tożsamości;
///  * w etapie 2 dodać [Authorize] / sprawdzenie uprawnień (admin, moderator forum, autor).
/// </summary>
public class ForumController : Controller
{
    private const int DefaultPageSize = 10;

    private readonly IForumRepository _repository;

    public ForumController(IForumRepository repository)
    {
        _repository = repository;
    }

    // --- Przegląd kategorii i forów ---------------------------------------

    public IActionResult Index()
    {
        var sections = _repository.GetCategories()
            .Select(c => new CategorySectionViewModel
            {
                Category = c,
                Forums = _repository.GetForumsByCategory(c.Id).Select(BuildForumCard).ToList()
            })
            .Where(s => s.Forums.Count > 0 || !string.IsNullOrEmpty(s.Category.Description))
            .ToList();

        ViewData["ActiveNav"] = "forum";
        return View(new ForumIndexViewModel { Sections = sections });
    }

    private ForumCardViewModel BuildForumCard(Forum forum)
    {
        var lastThread = _repository.GetThreads(forum.Id, 1, 1, null).Items.FirstOrDefault();
        return new ForumCardViewModel
        {
            Id = forum.Id,
            Name = forum.Name,
            Description = forum.Description,
            AllowAnonymousView = forum.AllowAnonymousView,
            AllowAnonymousPost = forum.AllowAnonymousPost,
            ThreadCount = _repository.GetThreads(forum.Id, 1, 1, null).TotalCount,
            MessageCount = _repository.CountMessagesInForum(forum.Id),
            ModeratorNames = forum.ModeratorIds
                .Select(id => _repository.GetUser(id)?.Username)
                .Where(name => !string.IsNullOrEmpty(name))
                .Cast<string>()
                .ToList(),
            LastThreadId = lastThread?.Id,
            LastThreadTitle = lastThread?.Title,
            LastThreadAt = lastThread?.LastActivityAt
        };
    }

    // --- Lista wątków ------------------------------------------------------

    public IActionResult Threads(int forumId, int page = 1, string? q = null)
    {
        var forum = _repository.GetForum(forumId);
        if (forum is null) return NotFound();

        var pageSize = ReadPageSizeFromCookie();
        var result = _repository.GetThreads(forumId, page, pageSize, q);

        var items = result.Items.Select(t => new ThreadListItemViewModel
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            AuthorName = t.AuthorName,
            IsPinned = t.IsPinned,
            CreatedAt = t.CreatedAt,
            LastActivityAt = t.LastActivityAt,
            ReplyCount = _repository.CountMessagesInThread(t.Id),
            ViewCount = t.ViewCount,
            LastAuthorName = _repository.GetLastMessage(t.Id)?.AuthorName
        }).ToList();

        var category = _repository.GetCategory(forum.CategoryId);

        var vm = new ThreadListViewModel
        {
            ForumId = forum.Id,
            ForumName = forum.Name,
            ForumDescription = forum.Description,
            CategoryName = category?.Name ?? string.Empty,
            Threads = items,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            TotalCount = result.TotalCount,
            Query = q
        };

        ViewData["ActiveNav"] = "forum";
        return View(vm);
    }

    // --- Strona wątku ------------------------------------------------------

    public IActionResult Thread(int id, int page = 1)
    {
        var thread = _repository.GetThread(id);
        if (thread is null) return NotFound();

        _repository.IncrementViewCount(id);
        thread = _repository.GetThread(id)!;

        var forum = _repository.GetForum(thread.ForumId);
        var category = forum is null ? null : _repository.GetCategory(forum.CategoryId);
        var pageSize = ReadPageSizeFromCookie();
        var messages = _repository.GetMessages(id, page, pageSize);
        var author = _repository.GetUserByUsername(thread.AuthorName);

        var vm = new ThreadDetailsViewModel
        {
            Id = thread.Id,
            ForumId = thread.ForumId,
            ForumName = forum?.Name ?? string.Empty,
            CategoryName = category?.Name ?? string.Empty,
            Title = thread.Title,
            Description = thread.Description,
            Content = thread.Content,
            AuthorName = thread.AuthorName,
            AuthorRank = author?.Rank ?? "Nowicjusz",
            AuthorJoinedAt = author?.JoinedAt ?? DateTime.MinValue,
            CreatedAt = thread.CreatedAt,
            ViewCount = thread.ViewCount,
            ReplyCount = _repository.CountMessagesInThread(id),
            IsPinned = thread.IsPinned,
            Page = messages.Page,
            PageSize = messages.PageSize,
            TotalPages = messages.TotalPages,
            Reply = new ReplyViewModel { ThreadId = thread.Id },
            Messages = messages.Items.Select(m => BuildMessageViewModel(m, author)).ToList()
        };

        ViewData["ActiveNav"] = "forum";
        return View(vm);
    }

    private MessageViewModel BuildMessageViewModel(ForumMessage message, UserAccount? threadAuthor)
    {
        var author = _repository.GetUserByUsername(message.AuthorName);
        return new MessageViewModel
        {
            Id = message.Id,
            AuthorName = message.AuthorName,
            AuthorRank = author?.Rank ?? "Nowicjusz",
            AuthorJoinedAt = author?.JoinedAt ?? DateTime.MinValue,
            AuthorMessageCount = author?.MessageCount ?? 0,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            EditedAt = message.EditedAt
        };
    }

    // --- Tworzenie wątku ---------------------------------------------------

    [HttpGet]
    public IActionResult CreateThread(int forumId)
    {
        var forums = _repository.GetForums();
        if (forums.Count == 0) return RedirectToAction(nameof(Index));

        var validForumId = forums.Any(f => f.Id == forumId) ? forumId : forums[0].Id;
        var vm = new CreateThreadViewModel { ForumId = validForumId };
        ViewData["Forums"] = new SelectList(
            forums.Select(f => new { f.Id, f.Name }), "Id", "Name", validForumId);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateThread(CreateThreadViewModel model)
    {
        var forums = _repository.GetForums();
        ViewData["Forums"] = new SelectList(
            forums.Select(f => new { f.Id, f.Name }), "Id", "Name", model.ForumId);

        if (!ModelState.IsValid) return View(model);

        if (_repository.GetForum(model.ForumId) is null)
        {
            ModelState.AddModelError(nameof(model.ForumId), "Wybrane forum nie istnieje.");
            return View(model);
        }

        // TODO (etap 2): IsPinned ustawia wyłącznie administrator — weryfikacja po stronie serwera.
        var thread = new ForumThread
        {
            ForumId = model.ForumId,
            Title = model.Title.Trim(),
            Description = model.Description?.Trim() ?? string.Empty,
            Content = model.Content.Trim(),
            AuthorName = ResolveAuthorName(),
            IsPinned = model.IsPinned
        };

        var id = _repository.AddThread(thread);
        TempData["Success"] = "Wątek został utworzony.";
        return RedirectToAction(nameof(Thread), new { id });
    }

    // --- Odpowiedź w wątku -------------------------------------------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reply(ReplyViewModel Reply)
    {
        // Parametr nazany „Reply” = prefiks bindowania dla pól formularza Reply.*.
        if (string.IsNullOrWhiteSpace(Reply.AuthorName))
        {
            // Brak sesji klienta (auth.js) — w etapie 2 tożsamość pochodzi z sesji serwerowej.
            TempData["Error"] = "Zaloguj się, aby dodać odpowiedź.";
            return RedirectToAction(nameof(Thread), new { id = Reply.ThreadId });
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Odpowiedź została odrzucona — uzupełnij treść (min. 2 znaki).";
            return RedirectToAction(nameof(Thread), new { id = Reply.ThreadId });
        }

        var thread = _repository.GetThread(Reply.ThreadId);
        if (thread is null) return NotFound();

        // TODO (etap 2): weryfikacja uprawnień do pisania w tym forum (AllowAnonymousPost / zalogowany).
        _repository.AddMessage(new ForumMessage
        {
            ThreadId = thread.Id,
            AuthorName = Reply.AuthorName.Trim(),
            Content = Reply.Content.Trim()
        });

        TempData["Success"] = "Odpowiedź została dodana.";
        return RedirectToAction(nameof(Thread), new { id = thread.Id });
    }

    // --- Edycja i usuwanie wiadomości (autor / moderator / admin) ---------

    [HttpGet]
    public IActionResult EditMessage(int id)
    {
        var message = _repository.GetMessage(id);
        if (message is null) return NotFound();

        // TODO (etap 2): sprawdzenie uprawnień — autor wiadomości, moderator forum lub administrator.
        var vm = new EditMessageViewModel { Id = message.Id, ThreadId = message.ThreadId, Content = message.Content };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditMessage(EditMessageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var existing = _repository.GetMessage(model.Id);
            if (existing is null) return NotFound();
            return View(model);
        }

        var message = _repository.GetMessage(model.Id);
        if (message is null) return NotFound();

        message.Content = model.Content.Trim();
        _repository.UpdateMessage(message);

        TempData["Success"] = "Wiadomość została zaktualizowana.";
        return RedirectToAction(nameof(Thread), new { id = model.ThreadId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteMessage(int id)
    {
        var message = _repository.GetMessage(id);
        if (message is null) return NotFound();

        // TODO (etap 2): weryfikacja uprawnień (moderator/admin/autor).
        _repository.DeleteMessage(id);
        TempData["Success"] = "Wiadomość została usunięta.";
        return RedirectToAction(nameof(Thread), new { id = message.ThreadId });
    }

    // --- Pomocnicze --------------------------------------------------------

    /// <summary>Nazwa autora pochodzi z formularza wypełnianego przez auth.js (tymczasowy pomost do etapu 2).</summary>
    private string ResolveAuthorName()
    {
        var author = Request.Form["AuthorName"].FirstOrDefault();
        return string.IsNullOrWhiteSpace(author) ? "Gość" : author.Trim();
    }

    /// <summary>Liczba wiadomości na stronie: cookie ustawiane w profilu (w etapie 2 — ustawienie konta).</summary>
    private int ReadPageSizeFromCookie()
    {
        if (Request.Cookies.TryGetValue("forum.pageSize", out var raw)
            && int.TryParse(raw, out var pageSize))
        {
            return Math.Clamp(pageSize, 5, 50);
        }
        return DefaultPageSize;
    }
}
