namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>Globalne role kont. Uprawnienia moderatora są przypisane per forum (patrz Forum.ModeratorIds).</summary>
public enum UserRole
{
    User = 0,
    Admin = 1
}
