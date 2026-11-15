namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>
/// Konto użytkownika widoczne w panelu administracyjnym i na forach.
/// W etapie 1 lista służy do demonstracji; uwierzytelnianie działa po stronie klienta (localStorage),
/// a w etapie 2 źródłem stanie się ASP.NET Identity.
/// </summary>
public class UserAccount
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>Ranga tekstowa; w etapie 2 naliczana automatycznie na podstawie liczby wiadomości.</summary>
    public string Rank { get; set; } = "Nowicjusz";

    public bool IsActive { get; set; } = true;
    public int MessageCount { get; set; }
    public DateTime JoinedAt { get; set; }
    public string? AvatarUrl { get; set; }
}
