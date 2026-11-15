using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Formularz rejestracji konta.</summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Podaj nazwę użytkownika.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 30 znaków.")]
    [Display(Name = "Nazwa użytkownika")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj adres e-mail.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
    [StringLength(100, ErrorMessage = "Adres e-mail może mieć maksymalnie 100 znaków.")]
    [Display(Name = "Adres e-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj hasło.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$",
        ErrorMessage = "Hasło musi zawierać co najmniej jedną literę i jedną cyfrę.")]
    [DataType(DataType.Password)]
    [Display(Name = "Hasło")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Powtórz hasło.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Hasła nie są identyczne.")]
    [Display(Name = "Powtórz hasło")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "Musisz zaakceptować regulamin, aby się zarejestrować.")]
    [Display(Name = "Akceptuję regulamin forum")]
    public bool AcceptTerms { get; set; }
}
