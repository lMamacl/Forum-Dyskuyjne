using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskusyjne.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 50 znaków.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć minimum {2} znaków.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
