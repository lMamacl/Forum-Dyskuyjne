using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Lista kategorii + formularz dodawania/edycji (admin).</summary>
public class CategoriesAdminViewModel
{
    public IReadOnlyList<Forum_Dyskuyjne.Models.Domain.Category> Categories { get; set; } = Array.Empty<Forum_Dyskuyjne.Models.Domain.Category>();

    /// <summary>Liczba forów w kategorii — pokazywana na liście, blokuje usuwanie zajętych kategorii.</summary>
    public IReadOnlyDictionary<int, int> ForumsByCategory { get; set; } = new Dictionary<int, int>();

    public CategoryFormViewModel Form { get; set; } = new();
}

public class CategoryFormViewModel
{
    /// <summary>0 = nowa kategoria.</summary>
    public int Id { get; set; }

    [Required(ErrorMessage = "Podaj nazwę kategorii.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Nazwa musi mieć od 2 do 60 znaków.")]
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "Opis może mieć maksymalnie 300 znaków.")]
    [Display(Name = "Opis")]
    public string Description { get; set; } = string.Empty;

    [Range(0, 999, ErrorMessage = "Kolejność musi być liczbą z zakresu 0–999.")]
    [Display(Name = "Kolejność wyświetlania")]
    public int Position { get; set; }
}
