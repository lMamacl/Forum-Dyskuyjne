namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Strona wątku: wiadomość inicjująca, stronicowane odpowiedzi, formularz odpowiedzi.</summary>
public class ThreadDetailsViewModel
{
    public int Id { get; set; }
    public int ForumId { get; set; }
    public string ForumName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorRank { get; set; } = "Nowicjusz";
    public DateTime AuthorJoinedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
    public int ReplyCount { get; set; }
    public bool IsPinned { get; set; }

    public IReadOnlyList<MessageViewModel> Messages { get; set; } = Array.Empty<MessageViewModel>();
    public ReplyViewModel Reply { get; set; } = new();

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;
}

public class MessageViewModel
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorRank { get; set; } = "Nowicjusz";
    public DateTime AuthorJoinedAt { get; set; }
    public int AuthorMessageCount { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
}
