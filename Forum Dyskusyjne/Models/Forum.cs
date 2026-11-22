namespace Forum_Dyskusyjne.Models;

public class Forum
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacja 1 do wielu: jedno forum ma wiele wątków
    public List<ForumThread> Threads { get; set; } = new();
}