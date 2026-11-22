namespace Forum_Dyskusyjne.Models;

public class ForumThread
{
    public int Id { get; set; }
    public int ForumId { get; set; }                    // Klucz obcy wskazujący na Forum
    public Forum? Forum { get; set; }                   // Właściwość nawigacyjna

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacja 1 do wielu: jeden wątek ma wiele odpowiedzi (postów)
    public List<ForumPost> Posts { get; set; } = new();
}