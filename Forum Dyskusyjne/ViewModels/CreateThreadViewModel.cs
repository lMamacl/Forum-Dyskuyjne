using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskusyjne.ViewModels;

public class CreateThreadViewModel
{
    // Id forum, w którym wątek jest zakładany (ukryte pole w formularzu)
    public int ForumId { get; set; }

    // Tylko do wyświetlenia w nagłówku formularza ("Nowy wątek w: Programowanie")
    public string? ForumName { get; set; }

    [Required(ErrorMessage = "Tytuł wątku jest wymagany.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Tytuł musi mieć od 5 do 100 znaków.")]
    [Display(Name = "Tytuł wątku")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Treść wpisu jest wymagana.")]
    [StringLength(4000, MinimumLength = 10, ErrorMessage = "Treść musi mieć co najmniej 10 znaków.")]
    [Display(Name = "Opis wątku")]
    public string Content { get; set; } = string.Empty;
}