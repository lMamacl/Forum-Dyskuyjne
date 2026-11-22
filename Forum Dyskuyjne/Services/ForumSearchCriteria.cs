using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Services;

/// <summary>
/// Kryteria wyszukiwania wątków na domyślnym widoku listy (globalna wyszukiwarka).
/// Wszystkie filtry są opcjonalne — puste kryteria zwracają pełną listę.
/// </summary>
public sealed class ForumSearchCriteria
{
    /// <summary>Fraza w tytule, opisie, treści wątku lub treściach jego wiadomości (dowolne słowo — tryb OR; tryb AND/OR/NOT w etapie 2).</summary>
    public string? Query { get; init; }

    /// <summary>Ograniczenie do jednego forum (null = wszystkie fora).</summary>
    public int? ForumId { get; init; }

    /// <summary>Ograniczenie do jednej kategorii wątku (null = wszystkie kategorie).</summary>
    public int? CategoryId { get; init; }

    public ThreadSort Sort { get; init; } = ThreadSort.Active;

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
