using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskusyjne.ViewModels;

public class CreatePostViewModel
{
    // ID wątku, do którego dodajemy odpowiedź
    public int ThreadId { get; set; }

    [Required(ErrorMessage = "Treść odpowiedzi nie może być pusta.")]
    [StringLength(2000, MinimumLength = 3, ErrorMessage = "Odpowiedź musi mieć od 3 do 2000 znaków.")]
    [Display(Name = "Twoja odpowiedź")]
    public string Content { get; set; } = string.Empty;
}