namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Lista wątków jednego forum: paginacja, wyszukiwarka, statystyki.</summary>
public class ThreadListViewModel
{
    public int ForumId { get; set; }
    public string ForumName { get; set; } = string.Empty;
    public string ForumDescription { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;

    public IReadOnlyList<ThreadListItemViewModel> Threads { get; set; } = Array.Empty<ThreadListItemViewModel>();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }
    public string? Query { get; set; }
}

public class ThreadListItemViewModel
{
    public int Id { get; set; }
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
