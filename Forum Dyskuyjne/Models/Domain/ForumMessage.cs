namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>Odpowiedź dodana do wątku. Edytować może autor, moderator forum lub administrator.</summary>
public class ForumMessage
{
    public int Id { get; set; }
    public int ThreadId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
}
