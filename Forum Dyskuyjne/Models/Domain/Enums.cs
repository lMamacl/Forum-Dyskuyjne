namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>Globalne role kont. Uprawnienia moderatora są przypisane per forum (patrz Forum.ModeratorIds).</summary>
public enum UserRole
{
    User = 0,
    Admin = 1
}

/// <summary>Sortowanie na domyślnym widoku listy wątków (wyszukiwarka).</summary>
public enum ThreadSort
{
    /// <summary>Najaktywniejsze — wg ostatniej aktywności (domyślne).</summary>
    Active = 0,

    /// <summary>Najnowsze — wg daty utworzenia.</summary>
    Newest = 1,

    /// <summary>Najwięcej odpowiedzi.</summary>
    Replies = 2,

    /// <summary>Najwięcej odsłon.</summary>
    Views = 3
}
