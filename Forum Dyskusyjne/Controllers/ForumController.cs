using Forum_Dyskusyjne.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum_Dyskusyjne.Controllers;

public class ForumController : Controller
{
    private readonly ForumDbContext _context;

    // Kontener ASP.NET automatycznie dostarcza zarejestrowany w Program.cs ForumDbContext
    public ForumController(ForumDbContext context)
    {
        _context = context;
    }

    // 1. Lista wszystkich forów: GET /Forum
    public async Task<IActionResult> Index()
    {
        // Pobieramy fora z bazy wraz z informacją o wątkach (żeby policzyć ile ich jest)
        var forums = await _context.Forums
            .Include(f => f.Threads)
            .OrderBy(f => f.Id)
            .ToListAsync();

        return View(forums);
    }
        // 2. Lista wątków w danym forum: GET /Forum/Threads?forumId=1
    public async Task<IActionResult> Threads(int forumId)
    {
        var forum = await _context.Forums
            .Include(f => f.Threads)
                .ThenInclude(t => t.Posts)
            .FirstOrDefaultAsync(f => f.Id == forumId);

        if (forum == null)
        {
            return NotFound("Nie znaleziono takiego forum.");
        }

        return View(forum);
    }
        // 3. Szczegóły wątku i posty: GET /Forum/Thread?id=1
    public async Task<IActionResult> Thread(int id)
    {
        var thread = await _context.Threads
            .Include(t => t.Forum)
            .Include(t => t.Posts)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (thread == null)
        {
            return NotFound("Nie znaleziono wątku.");
        }

        return View(thread);
    }
}