using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Formularz odpowiedzi na stronie wątku (edytor inline).</summary>
public class ReplyViewModel
{
    public int ThreadId { get; set; }

    /// <summary>Wypełniane z sesji klienta (auth.js); w etapie 2 pochodzi z uwierzytelnienia serwerowego.</summary>
    [StringLength(60)]
    public string? AuthorName { get; set; }

    [Required(ErrorMessage = "Odpowiedź nie może być pusta.")]
    [MinLength(2, ErrorMessage = "Odpowiedź musi mieć co najmniej 2 znaki.")]
    [Display(Name = "Treść odpowiedzi")]
    public string Content { get; set; } = string.Empty;
}
