using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Edycja treści wiadomości (autor, moderator forum, administrator).</summary>
public class EditMessageViewModel
{
    public int Id { get; set; }
    public int ThreadId { get; set; }

    [Required(ErrorMessage = "Treść nie może być pusta.")]
    [MinLength(2, ErrorMessage = "Treść musi mieć co najmniej 2 znaki.")]
    [Display(Name = "Treść wiadomości")]
    public string Content { get; set; } = string.Empty;
}
