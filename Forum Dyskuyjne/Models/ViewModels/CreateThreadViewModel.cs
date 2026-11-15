using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Formularz tworzenia nowego wątku.</summary>
public class CreateThreadViewModel
{
    [Required(ErrorMessage = "Wybierz forum.")]
    [Range(1, int.MaxValue, ErrorMessage = "Wybierz forum.")]
    [Display(Name = "Forum")]
    public int ForumId { get; set; }

    [Required(ErrorMessage = "Podaj tytuł wątku.")]
    [StringLength(120, MinimumLength = 5, ErrorMessage = "Tytuł musi mieć od 5 do 120 znaków.")]
    [Display(Name = "Tytuł")]
    public string Title { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Opis może mieć maksymalnie 200 znaków.")]
    [Display(Name = "Krótki opis (pod lista wątków)")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Wpisz treść wątku.")]
    [MinLength(10, ErrorMessage = "Treść musi mieć co najmniej 10 znaków.")]
    [Display(Name = "Treść")]
    public string Content { get; set; } = string.Empty;

    /// <summary>Dostępne tylko dla administratora — ukrywane w widoku dla pozostałych użytkowników.</summary>
    [Display(Name = "Przyklej wątek na górze listy")]
    public bool IsPinned { get; set; }
}
