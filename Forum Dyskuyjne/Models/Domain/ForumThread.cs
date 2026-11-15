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

    /// <summary>Wątek przyklejony — tylko administrator może ustawić; sortowany na górze listy.</summary>
    public bool IsPinned { get; set; }

    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
}
