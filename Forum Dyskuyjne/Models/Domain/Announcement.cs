namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>Ogłoszenie administracyjne wyświetlane na stronie głównej forum.</summary>
public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = "Administracja";
    public DateTime CreatedAt { get; set; }
}
