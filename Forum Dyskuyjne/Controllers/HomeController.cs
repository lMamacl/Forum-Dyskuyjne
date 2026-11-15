using Forum_Dyskuyjne.Models;
using Forum_Dyskuyjne.Models.ViewModels;
using Forum_Dyskuyjne.Services;
using Microsoft.AspNetCore.Mvc;

namespace Forum_Dyskuyjne.Controllers;

public class HomeController : Controller
{
    private readonly IForumRepository _repository;

    public HomeController(IForumRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Strona główna: powitanie, statystyki, ogłoszenia administracyjne, ostatnie wątki.</summary>
    public IActionResult Index()
    {
        var totals = _repository.GetTotals();

        var recent = _repository.GetRecentThreads(5)
            .Select(t => new ThreadListItemViewModel
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
            })
            .ToList();

        var vm = new HomeViewModel
        {
            TotalUsers = totals.Users,
            TotalThreads = totals.Threads,
            TotalMessages = totals.Messages,
            TotalForums = totals.Forums,
            Announcements = _repository.GetAnnouncements(),
            RecentThreads = recent
        };

        ViewData["ActiveNav"] = "home";
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id?.ToString() ?? HttpContext.TraceIdentifier
        });
    }
}
