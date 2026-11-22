using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Lista forów + formularz dodawania/edycji (admin): kategoria, uprawnienia anonimowe, moderatorzy.</summary>
public class ForumsAdminViewModel
{
    public IReadOnlyList<ForumAdminRowViewModel> Forums { get; set; } = Array.Empty<ForumAdminRowViewModel>();
    public ForumFormViewModel Form { get; set; } = new();

    /// <summary>Źródło listy wyboru kategorii (dokładnie jedna kategoria na forum).</summary>
    public IReadOnlyList<CategoryOptionViewModel> CategoryOptions { get; set; } = Array.Empty<CategoryOptionViewModel>();

    /// <summary>Źródło listy wyboru moderatorów.</summary>
    public IReadOnlyList<UserOptionViewModel> UserOptions { get; set; } = Array.Empty<UserOptionViewModel>();
}

public class ForumAdminRowViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public bool AllowAnonymousView { get; set; }
    public bool AllowAnonymousPost { get; set; }
    public int ThreadCount { get; set; }
    public int ModeratorCount { get; set; }
}

public class ForumFormViewModel
{
    /// <summary>0 = nowe forum.</summary>
    public int Id { get; set; }

    [Required(ErrorMessage = "Podaj nazwę forum.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Nazwa musi mieć od 2 do 60 znaków.")]
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "Opis może mieć maksymalnie 300 znaków.")]
    [Display(Name = "Opis")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Wybierz kategorię.")]
    [Range(1, int.MaxValue, ErrorMessage = "Wybierz kategorię.")]
    [Display(Name = "Kategoria (dokładnie jedna)")]
    public int CategoryId { get; set; }

    [Display(Name = "Anonimowie mogą przeglądać wiadomości")]
    public bool AllowAnonymousView { get; set; } = true;

    [Display(Name = "Anonimowie mogą pisać wiadomości")]
    public bool AllowAnonymousPost { get; set; }

    [Display(Name = "Moderatorzy forum")]
    public List<int> ModeratorIds { get; set; } = new();
}

public class CategoryOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UserOptionViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
}
