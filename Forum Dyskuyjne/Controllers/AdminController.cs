using Forum_Dyskuyjne.Models.Domain;
using Forum_Dyskuyjne.Models.ViewModels;
using Forum_Dyskuyjne.Services;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Dyskuyjne.Controllers;

/// <summary>
/// Panel administracyjny: użytkownicy, kategorie, fora, ogłoszenia.
/// Etap 1: akcje zapisu działają na repozytorium in-memory.
/// Etap 2: [Authorize(Roles = nameof(UserRole.Admin))] + weryfikacja po stronie serwera.
/// </summary>
public class AdminController : Controller
{
    private readonly IForumRepository _repository;

    public AdminController(IForumRepository repository)
    {
        _repository = repository;
    }

    // --- Pulpit ------------------------------------------------------------

    public IActionResult Index()
    {
        var totals = _repository.GetTotals();
        var vm = new AdminDashboardViewModel
        {
            TotalUsers = totals.Users,
            TotalThreads = totals.Threads,
            TotalMessages = totals.Messages,
            TotalForums = totals.Forums,
            TotalCategories = totals.Categories,
            TotalAnnouncements = _repository.GetAnnouncements().Count,
            RecentUsers = _repository.GetUsers().OrderByDescending(u => u.JoinedAt).Take(5).ToList()
        };
        return View(vm);
    }

    // --- Użytkownicy -------------------------------------------------------

    public IActionResult Users(int? edit)
    {
        var users = _repository.GetUsers();
        var vm = new UsersAdminViewModel { Users = users };

        if (edit is > 0)
        {
            var user = users.FirstOrDefault(u => u.Id == edit.Value);
            if (user is not null)
            {
                vm.Form = new UserEditViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Rank = user.Rank,
                    IsActive = user.IsActive
                };
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditUser(UserEditViewModel Form)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Sprawdź formularz edycji użytkownika.";
            return RedirectToAction(nameof(Users), Form.Id > 0 ? new { edit = Form.Id } : null);
        }

        var user = _repository.GetUser(Form.Id);
        if (user is null) return NotFound();

        user.Username = Form.Username.Trim();
        user.Email = Form.Email.Trim();
        user.Role = Form.Role;
        user.Rank = Form.Rank.Trim();
        user.IsActive = Form.IsActive;
        _repository.UpdateUser(user);

        TempData["Success"] = $"Zaktualizowano konto „{user.Username}”.";
        return RedirectToAction(nameof(Users));
    }

    // --- Kategorie ---------------------------------------------------------

    public IActionResult Categories(int? edit)
    {
        var forums = _repository.GetForums();
        var vm = new CategoriesAdminViewModel
        {
            Categories = _repository.GetCategories(),
            ForumsByCategory = forums
                .GroupBy(f => f.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count())
        };
        if (edit is > 0)
        {
            var category = _repository.GetCategory(edit.Value);
            if (category is not null)
            {
                vm.Form = new CategoryFormViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Position = category.Position
                };
            }
        }
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveCategory(CategoryFormViewModel Form)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Sprawdź formularz kategorii.";
            return RedirectToAction(nameof(Categories), Form.Id > 0 ? new { edit = Form.Id } : null);
        }

        if (Form.Id == 0)
        {
            _repository.AddCategory(new Category
            {
                Name = Form.Name.Trim(),
                Description = Form.Description?.Trim() ?? string.Empty,
                Position = Form.Position
            });
            TempData["Success"] = "Kategoria została dodana.";
        }
        else
        {
            var category = _repository.GetCategory(Form.Id);
            if (category is null) return NotFound();
            category.Name = Form.Name.Trim();
            category.Description = Form.Description?.Trim() ?? string.Empty;
            category.Position = Form.Position;
            _repository.UpdateCategory(category);
            TempData["Success"] = "Kategoria została zaktualizowana.";
        }

        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteCategory(int id)
    {
        if (_repository.DeleteCategory(id))
        {
            TempData["Success"] = "Kategoria została usunięta.";
        }
        else
        {
            TempData["Error"] = "Nie można usunąć kategorii, która zawiera fora. Najpierw przenieś lub usuń fora.";
        }
        return RedirectToAction(nameof(Categories));
    }

    // --- Fora --------------------------------------------------------------

    public IActionResult Forums(int? edit)
    {
        var categories = _repository.GetCategories();
        var users = _repository.GetUsers();

        var vm = new ForumsAdminViewModel
        {
            Forums = _repository.GetForums().Select(f => new ForumAdminRowViewModel
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                CategoryName = categories.FirstOrDefault(c => c.Id == f.CategoryId)?.Name ?? "—",
                AllowAnonymousView = f.AllowAnonymousView,
                AllowAnonymousPost = f.AllowAnonymousPost,
                ThreadCount = _repository.GetThreads(f.Id, 1, 1, null).TotalCount,
                ModeratorCount = f.ModeratorIds.Count
            }).ToList(),
            CategoryOptions = categories.Select(c => new CategoryOptionViewModel { Id = c.Id, Name = c.Name }).ToList(),
            UserOptions = users.Select(u => new UserOptionViewModel { Id = u.Id, Username = u.Username }).ToList()
        };

        if (edit is > 0)
        {
            var forum = _repository.GetForum(edit.Value);
            if (forum is not null)
            {
                vm.Form = new ForumFormViewModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Description = forum.Description,
                    CategoryId = forum.CategoryId,
                    AllowAnonymousView = forum.AllowAnonymousView,
                    AllowAnonymousPost = forum.AllowAnonymousPost,
                    ModeratorIds = forum.ModeratorIds
                };
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveForum(ForumFormViewModel Form)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Sprawdź formularz forum.";
            return RedirectToAction(nameof(Forums), Form.Id > 0 ? new { edit = Form.Id } : null);
        }

        if (_repository.GetCategory(Form.CategoryId) is null)
        {
            TempData["Error"] = "Wybrana kategoria nie istnieje.";
            return RedirectToAction(nameof(Forums));
        }

        if (Form.Id == 0)
        {
            _repository.AddForum(new Forum
            {
                Name = Form.Name.Trim(),
                Description = Form.Description?.Trim() ?? string.Empty,
                CategoryId = Form.CategoryId,
                AllowAnonymousView = Form.AllowAnonymousView,
                AllowAnonymousPost = Form.AllowAnonymousPost,
                ModeratorIds = Form.ModeratorIds ?? new List<int>()
            });
            TempData["Success"] = "Forum zostało dodane.";
        }
        else
        {
            var forum = _repository.GetForum(Form.Id);
            if (forum is null) return NotFound();
            forum.Name = Form.Name.Trim();
            forum.Description = Form.Description?.Trim() ?? string.Empty;
            forum.CategoryId = Form.CategoryId;
            forum.AllowAnonymousView = Form.AllowAnonymousView;
            forum.AllowAnonymousPost = Form.AllowAnonymousPost;
            forum.ModeratorIds = Form.ModeratorIds ?? new List<int>();
            _repository.UpdateForum(forum);
            TempData["Success"] = "Forum zostało zaktualizowane.";
        }

        return RedirectToAction(nameof(Forums));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteForum(int id)
    {
        _repository.DeleteForum(id);
        TempData["Success"] = "Forum wraz z wątkami zostało usunięte.";
        return RedirectToAction(nameof(Forums));
    }

    // --- Ogłoszenia --------------------------------------------------------

    public IActionResult Announcements()
    {
        var vm = new AnnouncementsAdminViewModel { Announcements = _repository.GetAnnouncements() };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveAnnouncement(AnnouncementFormViewModel Form)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Sprawdź formularz ogłoszenia.";
            return RedirectToAction(nameof(Announcements));
        }

        _repository.AddAnnouncement(new Announcement
        {
            Title = Form.Title.Trim(),
            Content = Form.Content.Trim(),
            AuthorName = "admin" // TODO (etap 2): nazwa z sesji administratora
        });
        TempData["Success"] = "Ogłoszenie zostało opublikowane.";
        return RedirectToAction(nameof(Announcements));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteAnnouncement(int id)
    {
        _repository.DeleteAnnouncement(id);
        TempData["Success"] = "Ogłoszenie zostało usunięte.";
        return RedirectToAction(nameof(Announcements));
    }
}
