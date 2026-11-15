using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Formularz logowania.</summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Podaj adres e-mail.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
    [Display(Name = "Adres e-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj hasło.")]
    [DataType(DataType.Password)]
    [Display(Name = "Hasło")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Zapamiętaj mnie")]
    public bool RememberMe { get; set; }
}
