using System.ComponentModel.DataAnnotations;
using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Widok profilu użytkownika: karta konta, statystyki, ostatnie wypowiedzi.</summary>
public class ProfileViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Rank { get; set; } = "Nowicjusz";
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; }
    public int ThreadCount { get; set; }
    public int MessageCount { get; set; }
    public IReadOnlyList<ProfilePostViewModel> RecentPosts { get; set; } = Array.Empty<ProfilePostViewModel>();

    /// <summary>Awatar przechowywany w etapie 1 po stronie klienta; wypełniany przez auth.js.</summary>
    public string? AvatarUrl { get; set; }
}

public class ProfilePostViewModel
{
    public int ThreadId { get; set; }
    public string ThreadTitle { get; set; } = string.Empty;
    public int ForumId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
