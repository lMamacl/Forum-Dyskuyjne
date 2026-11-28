using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskusyjne.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 50 znaków.")]
    [Display(Name = "Nazwa Użytkownika")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    [StringLength(150)]
    [Display(Name = "Email")]
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
}
