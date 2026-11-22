namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>
/// Lista wątków — widok po kliknięciu forum (kontekst) albo globalny („Wszystkie wątki”).
/// Filtry panelu: fraza, kategoria wątku, sortowanie; paginacja serwerowa.
/// Forum nie jest selectem w panelu — pochodzi z kontekstu nawigacji (Browse → forum).
/// </summary>
public class ThreadListViewModel
{
    // --- Aktywne filtry ----------------------------------------------------
    /// <summary>Filtr forum — select w panelu; preselekcja po kliknięciu w przeglądzie Fora.</summary>
    public int? ForumId { get; set; }
    public int? CategoryId { get; set; }
    public string Sort { get; set; } = "active";
    public string? Query { get; set; }

    /// <summary>Nazwa forum — wypełniana tylko gdy filtr forum jest aktywny.</summary>
    public string? ForumName { get; set; }

    /// <summary>Opis forum — wypełniany tylko gdy filtr forum jest aktywny.</summary>
    public string? ForumDescription { get; set; }

    /// <summary>Nazwa kategorii wątku — wypełniana tylko gdy filtr kategorii jest aktywny.</summary>
    public string? CategoryName { get; set; }

    public bool HasActiveFilters =>
        ForumId.HasValue || CategoryId.HasValue || !string.IsNullOrWhiteSpace(Query) || Sort != "active";

    // --- Wyniki ------------------------------------------------------------
    public IReadOnlyList<ThreadListItemViewModel> Threads { get; set; } = Array.Empty<ThreadListItemViewModel>();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }

    // --- Źródła panelu filtrów ---------------------------------------------
    public IReadOnlyList<ForumOptionViewModel> Forums { get; set; } = Array.Empty<ForumOptionViewModel>();
    public IReadOnlyList<CategoryOptionViewModel> Categories { get; set; } = Array.Empty<CategoryOptionViewModel>();
}

/// <summary>Wiersz na liście wątków.</summary>
public class ThreadListItemViewModel
{
    public int Id { get; set; }
    public int ForumId { get; set; }

    /// <summary>Nazwa forum — widoczna na liście globalnej (gdy nie aktywny filtr forum).</summary>
    public string ForumName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int ReplyCount { get; set; }
    public int ViewCount { get; set; }
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public string? LastAuthorName { get; set; }
}

/// <summary>Pozycja listy wyboru forum (panel filtrów, formularz tworzenia wątku).</summary>
public class ForumOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
