using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Strona główna forum: kategorie z forami i ich statystykami.</summary>
public class ForumIndexViewModel
{
    public IReadOnlyList<CategorySectionViewModel> Sections { get; set; } = Array.Empty<CategorySectionViewModel>();
}

public class CategorySectionViewModel
{
    public Category Category { get; set; } = new();
    public IReadOnlyList<ForumCardViewModel> Forums { get; set; } = Array.Empty<ForumCardViewModel>();
}

public class ForumCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ThreadCount { get; set; }
    public int MessageCount { get; set; }
    public bool AllowAnonymousView { get; set; }
    public bool AllowAnonymousPost { get; set; }
    public IReadOnlyList<string> ModeratorNames { get; set; } = Array.Empty<string>();
    public int? LastThreadId { get; set; }
    public string? LastThreadTitle { get; set; }
    public DateTime? LastThreadAt { get; set; }
}
