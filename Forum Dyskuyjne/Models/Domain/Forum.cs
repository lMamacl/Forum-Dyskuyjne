namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>
/// Forum dyskusyjne należące do dokładnie jednej kategorii.
/// Moderatorzy przechowywani jako lista identyfikatorów użytkowników (w bazie: tabela przejściowa wiele-do-wielu).
/// </summary>
public class Forum
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }

    /// <summary>Kolejność wyświetlania w obrębie kategorii.</summary>
    public int Position { get; set; }

    /// <summary>Uprawnienie anonimowe: podgląd wiadomości.</summary>
    public bool AllowAnonymousView { get; set; } = true;

    /// <summary>Uprawnienie anonimowe: pisanie własnych wiadomości.</summary>
    public bool AllowAnonymousPost { get; set; }

    /// <summary>Identyfikatory użytkowników uprawnionych do moderacji tego forum (wielu-do-wielu).</summary>
    public List<int> ModeratorIds { get; set; } = new();
}
