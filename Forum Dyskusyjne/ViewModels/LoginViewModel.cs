using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskusyjne.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    [Display(Name = "Nazwa Użytkownika")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    [Display(Name = "Hasło")]
    public string Password { get; set; } = string.Empty;
    
    [Display(Name = "Zapamiętaj mnie")]
    public bool RememberMe { get; set; }
}
