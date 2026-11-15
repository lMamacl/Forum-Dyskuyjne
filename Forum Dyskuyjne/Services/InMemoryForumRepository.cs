using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Services;

/// <summary>
/// Implementacja repozytorium na pamięci roboczej, zasilana <see cref="SeedData"/>.
/// Działa do końca życia procesu — służy etapowi 1 (widoki i formularze) bez bazy danych.
/// Docelowa implementacja (EF Core) podmienia tę klasę w DI bez zmian w kontrolerach.
/// </summary>
public sealed class InMemoryForumRepository : IForumRepository
{
    private readonly object _gate = new();

    private readonly List<Category> _categories;
    private readonly List<Forum> _forums;
    private readonly List<ForumThread> _threads;
    private readonly List<ForumMessage> _messages;
    private readonly List<UserAccount> _users;
    private readonly List<Announcement> _announcements;

    private int _nextCategoryId;
    private int _nextForumId;
    private int _nextThreadId;
    private int _nextMessageId;
    private int _nextUserId;
    private int _nextAnnouncementId;

    public InMemoryForumRepository()
    {
        var seed = SeedData.Build();
        _categories = seed.Categories;
        _forums = seed.Forums;
        _threads = seed.Threads;
        _messages = seed.Messages;
        _users = seed.Users;
        _announcements = seed.Announcements;

        _nextCategoryId = _categories.Count == 0 ? 1 : _categories.Max(c => c.Id) + 1;
        _nextForumId = _forums.Count == 0 ? 1 : _forums.Max(f => f.Id) + 1;
        _nextThreadId = _threads.Count == 0 ? 1 : _threads.Max(t => t.Id) + 1;
        _nextMessageId = _messages.Count == 0 ? 1 : _messages.Max(m => m.Id) + 1;
        _nextUserId = _users.Count == 0 ? 1 : _users.Max(u => u.Id) + 1;
        _nextAnnouncementId = _announcements.Count == 0 ? 1 : _announcements.Max(a => a.Id) + 1;
    }

    // --- Kategorie ---------------------------------------------------------

    public IReadOnlyList<Category> GetCategories()
    {
        lock (_gate) return _categories.OrderBy(c => c.Position).ThenBy(c => c.Id).ToList();
    }

    public Category? GetCategory(int id)
    {
        lock (_gate) return _categories.FirstOrDefault(c => c.Id == id);
    }

    public void AddCategory(Category category)
    {
        lock (_gate)
        {
            category.Id = _nextCategoryId++;
            _categories.Add(category);
        }
    }

    public void UpdateCategory(Category category)
    {
        lock (_gate)
        {
            var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing is null) return;
            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.Position = category.Position;
        }
    }

    public bool DeleteCategory(int id)
    {
        lock (_gate)
        {
            if (_forums.Any(f => f.CategoryId == id)) return false;
            return _categories.RemoveAll(c => c.Id == id) > 0;
        }
    }

    // --- Fora --------------------------------------------------------------

    public IReadOnlyList<Forum> GetForums()
    {
        lock (_gate) return _forums.OrderBy(f => f.Position).ThenBy(f => f.Id).ToList();
    }

    public Forum? GetForum(int id)
    {
        lock (_gate) return _forums.FirstOrDefault(f => f.Id == id);
    }

    public IReadOnlyList<Forum> GetForumsByCategory(int categoryId)
    {
        lock (_gate)
            return _forums.Where(f => f.CategoryId == categoryId)
                .OrderBy(f => f.Position).ThenBy(f => f.Id).ToList();
    }

    public void AddForum(Forum forum)
    {
        lock (_gate)
        {
            forum.Id = _nextForumId++;
            _forums.Add(forum);
        }
    }

    public void UpdateForum(Forum forum)
    {
        lock (_gate)
        {
            var existing = _forums.FirstOrDefault(f => f.Id == forum.Id);
            if (existing is null) return;
            existing.Name = forum.Name;
            existing.Description = forum.Description;
            existing.CategoryId = forum.CategoryId;
            existing.AllowAnonymousView = forum.AllowAnonymousView;
            existing.AllowAnonymousPost = forum.AllowAnonymousPost;
            existing.ModeratorIds = forum.ModeratorIds;
        }
    }

    public void DeleteForum(int id)
    {
        lock (_gate)
        {
            var threadIds = _threads.Where(t => t.ForumId == id).Select(t => t.Id).ToHashSet();
            _messages.RemoveAll(m => threadIds.Contains(m.ThreadId));
            _threads.RemoveAll(t => t.ForumId == id);
            _forums.RemoveAll(f => f.Id == id);
        }
    }

    // --- Wątki -------------------------------------------------------------

    public PagedResult<ForumThread> GetThreads(int forumId, int page, int pageSize, string? query)
    {
        lock (_gate)
        {
            var query2 = query?.Trim();
            IEnumerable<ForumThread> result = _threads.Where(t => t.ForumId == forumId);

            if (!string.IsNullOrEmpty(query2))
            {
                var terms = query2.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                result = result.Where(t => terms.All(term => MatchesThread(t, term)));
            }

            var ordered = result
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.LastActivityAt)
                .ToList();

            return Page(ordered, page, pageSize);
        }
    }

    private bool MatchesThread(ForumThread thread, string term) =>
        Contains(thread.Title, term)
        || Contains(thread.Description, term)
        || Contains(thread.Content, term)
        || _messages.Any(m => m.ThreadId == thread.Id && Contains(m.Content, term));

    private static bool Contains(string source, string term) =>
        source.Contains(term, StringComparison.OrdinalIgnoreCase);

    public ForumThread? GetThread(int id)
    {
        lock (_gate) return _threads.FirstOrDefault(t => t.Id == id);
    }

    public int AddThread(ForumThread thread)
    {
        lock (_gate)
        {
            thread.Id = _nextThreadId++;
            thread.CreatedAt = DateTime.UtcNow;
            thread.LastActivityAt = thread.CreatedAt;
            thread.ViewCount = 0;
            _threads.Add(thread);
            return thread.Id;
        }
    }

    public int CountThreadsByAuthor(string authorName)
    {
        lock (_gate)
            return _threads.Count(t => string.Equals(t.AuthorName, authorName, StringComparison.OrdinalIgnoreCase));
    }

    public void UpdateThread(ForumThread thread)
    {
        lock (_gate)
        {
            var existing = _threads.FirstOrDefault(t => t.Id == thread.Id);
            if (existing is null) return;
            existing.Title = thread.Title;
            existing.Description = thread.Description;
            existing.Content = thread.Content;
            existing.IsPinned = thread.IsPinned;
        }
    }

    public bool DeleteThread(int id)
    {
        lock (_gate)
        {
            _messages.RemoveAll(m => m.ThreadId == id);
            return _threads.RemoveAll(t => t.Id == id) > 0;
        }
    }

    public void IncrementViewCount(int threadId)
    {
        lock (_gate)
        {
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            if (thread is not null) thread.ViewCount++;
        }
    }

    public IReadOnlyList<ForumThread> GetRecentThreads(int count)
    {
        lock (_gate)
            return _threads.OrderByDescending(t => t.LastActivityAt).Take(count).ToList();
    }

    // --- Wiadomości --------------------------------------------------------

    public PagedResult<ForumMessage> GetMessages(int threadId, int page, int pageSize)
    {
        lock (_gate)
        {
            var ordered = _messages.Where(m => m.ThreadId == threadId)
                .OrderBy(m => m.CreatedAt)
                .ThenBy(m => m.Id)
                .ToList();
            return Page(ordered, page, pageSize);
        }
    }

    public IReadOnlyList<ForumMessage> GetRecentMessagesByAuthor(string authorName, int count)
    {
        lock (_gate)
            return _messages.Where(m => string.Equals(m.AuthorName, authorName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(m => m.CreatedAt)
                .Take(count)
                .ToList();
    }

    public ForumMessage? GetMessage(int id)
    {
        lock (_gate) return _messages.FirstOrDefault(m => m.Id == id);
    }

    public ForumMessage? GetLastMessage(int threadId)
    {
        lock (_gate)
            return _messages.Where(m => m.ThreadId == threadId)
                .OrderByDescending(m => m.CreatedAt)
                .ThenByDescending(m => m.Id)
                .FirstOrDefault();
    }

    public void AddMessage(ForumMessage message)
    {
        lock (_gate)
        {
            message.Id = _nextMessageId++;
            message.CreatedAt = DateTime.UtcNow;
            message.EditedAt = null;
            _messages.Add(message);

            var thread = _threads.FirstOrDefault(t => t.Id == message.ThreadId);
            if (thread is not null) thread.LastActivityAt = message.CreatedAt;

            var author = _users.FirstOrDefault(u => string.Equals(u.Username, message.AuthorName, StringComparison.OrdinalIgnoreCase));
            if (author is not null) author.MessageCount++;
        }
    }

    public void UpdateMessage(ForumMessage message)
    {
        lock (_gate)
        {
            var existing = _messages.FirstOrDefault(m => m.Id == message.Id);
            if (existing is null) return;
            existing.Content = message.Content;
            existing.EditedAt = DateTime.UtcNow;
        }
    }

    public bool DeleteMessage(int id)
    {
        lock (_gate) return _messages.RemoveAll(m => m.Id == id) > 0;
    }

    public int CountMessagesInThread(int threadId)
    {
        lock (_gate) return _messages.Count(m => m.ThreadId == threadId);
    }

    public int CountMessagesInForum(int forumId)
    {
        lock (_gate)
        {
            var ids = _threads.Where(t => t.ForumId == forumId).Select(t => t.Id).ToHashSet();
            return _messages.Count(m => ids.Contains(m.ThreadId));
        }
    }

    // --- Użytkownicy -------------------------------------------------------

    public IReadOnlyList<UserAccount> GetUsers()
    {
        lock (_gate) return _users.OrderBy(u => u.Username).ToList();
    }

    public UserAccount? GetUser(int id)
    {
        lock (_gate) return _users.FirstOrDefault(u => u.Id == id);
    }

    public UserAccount? GetUserByUsername(string username)
    {
        lock (_gate)
            return _users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public UserAccount? GetUserByEmail(string email)
    {
        lock (_gate)
            return _users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public void AddUser(UserAccount user)
    {
        lock (_gate)
        {
            user.Id = _nextUserId++;
            if (user.JoinedAt == default) user.JoinedAt = DateTime.UtcNow;
            _users.Add(user);
        }
    }

    public void UpdateUser(UserAccount user)
    {
        lock (_gate)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing is null) return;
            existing.Username = user.Username;
            existing.Email = user.Email;
            existing.Role = user.Role;
            existing.Rank = user.Rank;
            existing.IsActive = user.IsActive;
        }
    }

    public int CountUsers()
    {
        lock (_gate) return _users.Count;
    }

    // --- Ogłoszenia --------------------------------------------------------

    public IReadOnlyList<Announcement> GetAnnouncements()
    {
        lock (_gate) return _announcements.OrderByDescending(a => a.CreatedAt).ToList();
    }

    public void AddAnnouncement(Announcement announcement)
    {
        lock (_gate)
        {
            announcement.Id = _nextAnnouncementId++;
            announcement.CreatedAt = DateTime.UtcNow;
            _announcements.Add(announcement);
        }
    }

    public bool DeleteAnnouncement(int id)
    {
        lock (_gate) return _announcements.RemoveAll(a => a.Id == id) > 0;
    }

    // --- Statystyki --------------------------------------------------------

    public (int Users, int Threads, int Messages, int Forums, int Categories) GetTotals()
    {
        lock (_gate)
            return (_users.Count, _threads.Count, _messages.Count, _forums.Count, _categories.Count);
    }

    // --- Pomocnicze --------------------------------------------------------

    private static PagedResult<T> Page<T>(List<T> ordered, int page, int pageSize)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = ordered.Count;
        page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(total / (double)pageSize)));
        var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PagedResult<T>(items, page, pageSize, total);
    }
}
