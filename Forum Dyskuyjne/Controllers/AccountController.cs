using Forum_Dyskuyjne.Models.ViewModels;
using Forum_Dyskuyjne.Services;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Dyskuyjne.Controllers;

/// <summary>
/// Kontroler kont w etapie 1 udostępnia wyłącznie GET-i widoków — logowanie, rejestracja
/// i zapis ustawień działają przez moduł auth.js (localStorage), zgodnie z przyjętą decyzją
/// etapową (patrz docs/01-analiza-wymagan.md, sekcja 4.3).
///
/// Etap 2: akcje POST + ASP.NET Identity (walidacja ModelState pozostaje bez zmian).
/// </summary>
public class AccountController : Controller
{
    private readonly IForumRepository _repository;

    public AccountController(IForumRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null, string? reason = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["Reason"] = reason;
        return View(new LoginViewModel());
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    /// <summary>
    /// Profil. Parametr „name” wskazuje konto — w etapie 2 zostanie zastąpiony
    /// bieżącą sesją serwerową ([Authorize]). Bez parametru widok jest uzupełniany
    /// przez auth.js, które kieruje użytkownika na właściwy adres.
    /// </summary>
    [HttpGet]
    public IActionResult Profile(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return View(new ProfileViewModel());
        }

        var account = _repository.GetUserByUsername(name);
        var recentPosts = _repository.GetRecentMessagesByAuthor(name, 5)
            .Select(m =>
            {
                var thread = _repository.GetThread(m.ThreadId);
                return new ProfilePostViewModel
                {
                    ThreadId = m.ThreadId,
                    ThreadTitle = thread?.Title ?? "(usunięty wątek)",
                    ForumId = thread?.ForumId ?? 0,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                };
            })
            .ToList();

        var vm = new ProfileViewModel
        {
            Id = account?.Id ?? 0,
            Username = account?.Username ?? name,
            Email = account?.Email ?? string.Empty, // uzupełniane po stronie klienta dla właściciela profilu
            Role = account?.Role ?? Models.Domain.UserRole.User,
            Rank = account?.Rank ?? "Nowicjusz",
            IsActive = account?.IsActive ?? true,
            JoinedAt = account?.JoinedAt ?? DateTime.MinValue,
            ThreadCount = _repository.CountThreadsByAuthor(name),
            MessageCount = account?.MessageCount ?? 0,
            RecentPosts = recentPosts
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Settings()
    {
        // Formularz jest wypełniany danymi z sesji po stronie klienta (account.js).
        return View(new SettingsViewModel());
    }
}
