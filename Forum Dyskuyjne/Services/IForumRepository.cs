using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Services;

/// <summary>
/// Jedyne okno do danych forum. W etapie 1 implementacja: <see cref="InMemoryForumRepository"/> (dane demonstracyjne),
/// w etapie 2: implementacja oparta o EF Core — kontrolery i widoki pozostają bez zmian.
/// </summary>
public interface IForumRepository
{
    // --- Kategorie i fora -------------------------------------------------
    IReadOnlyList<Category> GetCategories();
    Category? GetCategory(int id);
    void AddCategory(Category category);
    void UpdateCategory(Category category);
    bool DeleteCategory(int id); // false, gdy kategoria zawiera fora

    IReadOnlyList<Forum> GetForums();
    Forum? GetForum(int id);
    IReadOnlyList<Forum> GetForumsByCategory(int categoryId);
    void AddForum(Forum forum);
    void UpdateForum(Forum forum);
    void DeleteForum(int id); // kaskada: wątki i wiadomości foru

    // --- Wątki ------------------------------------------------------------
    /// <summary>Wyszukiwanie i filtrowanie wątków — źródło danych domyślnego widoku listy.</summary>
    PagedResult<ForumThread> SearchThreads(ForumSearchCriteria criteria);
    ForumThread? GetThread(int id);
    int AddThread(ForumThread thread); // zwraca identyfikator nowego wątku
    int CountThreadsByAuthor(string authorName);
    int CountThreadsInForum(int forumId);
    int CountThreadsByCategory(int categoryId);
    void UpdateThread(ForumThread thread);
    bool DeleteThread(int id);
    void IncrementViewCount(int threadId);
    IReadOnlyList<ForumThread> GetRecentThreads(int count);

    // --- Wiadomości -------------------------------------------------------
    PagedResult<ForumMessage> GetMessages(int threadId, int page, int pageSize);
    IReadOnlyList<ForumMessage> GetRecentMessagesByAuthor(string authorName, int count);
    ForumMessage? GetMessage(int id);
    ForumMessage? GetLastMessage(int threadId);
    void AddMessage(ForumMessage message); // aktualizuje też LastActivityAt wątku
    void UpdateMessage(ForumMessage message);
    bool DeleteMessage(int id);
    int CountMessagesInThread(int threadId);
    int CountMessagesInForum(int forumId);

    // --- Użytkownicy ------------------------------------------------------
    IReadOnlyList<UserAccount> GetUsers();
    UserAccount? GetUser(int id);
    UserAccount? GetUserByUsername(string username);
    UserAccount? GetUserByEmail(string email);
    void AddUser(UserAccount user);
    void UpdateUser(UserAccount user);
    int CountUsers();

    // --- Ogłoszenia -------------------------------------------------------
    IReadOnlyList<Announcement> GetAnnouncements();
    void AddAnnouncement(Announcement announcement);
    bool DeleteAnnouncement(int id);

    // --- Statystyki ogólne ------------------------------------------------
    (int Users, int Threads, int Messages, int Forums, int Categories) GetTotals();
}
