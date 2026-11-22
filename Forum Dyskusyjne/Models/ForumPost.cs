namespace Forum_Dyskusyjne.Models;

public class ForumPost
{
    public int Id { get; set; }
    public int ThreadId { get; set; }                   // Klucz obcy wskazujący na Wątek
    public ForumThread? Thread { get; set; }            // Właściwość nawigacyjna

    public string Content { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}