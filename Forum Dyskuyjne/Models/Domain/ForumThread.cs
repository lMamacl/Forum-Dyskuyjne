namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>
/// Wątek: wiadomość inicjująca (Title/Description/Content) plus odpowiadające na nią wiadomości.
/// AuthorName jest literałem (w T1 konta żyją po stronie klienta); w T2 zastąpione kluczem obcym na użytkownika.
/// </summary>
public class ForumThread
{
    public int Id { get; set; }
    public int ForumId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Kategorie wątku (tagi) — wiele, nadawanych przy tworzeniu; służą do filtrowania na liście.
    /// Nie mylić z kategorią fora (<see cref="Forum.CategoryId"/>): każde forum należy do dokładnie
    /// jednej kategorii, a jeden wątek może być opisany wieloma kategoriami ze wspólnego słownika.
    /// W T2: tabela łącząca ForumThreadCategories(ForumThreadId, CategoryId) — relacja wiele-do-wielu.
    /// </summary>
    public List<int> CategoryIds { get; set; } = new();

    /// <summary>Wątek przyklejony — tylko administrator może ustawić; sortowany na górze listy wątków forum.</summary>
    public bool IsPinned { get; set; }

    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
}
