using System.ComponentModel.DataAnnotations;
using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Lista użytkowników w panelu administratora.</summary>
public class UsersAdminViewModel
{
    public IReadOnlyList<UserAccount> Users { get; set; } = Array.Empty<UserAccount>();
    public UserEditViewModel Form { get; set; } = new();
}

/// <summary>Edycja konta: rola, ranga (ręczna zmiana), aktywność.</summary>
public class UserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Podaj nazwę użytkownika.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 30 znaków.")]
    [Display(Name = "Nazwa użytkownika")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj adres e-mail.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
    [Display(Name = "Adres e-mail")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Rola")]
    public UserRole Role { get; set; } = UserRole.User;

    [Required(ErrorMessage = "Podaj rangę.")]
    [StringLength(40, ErrorMessage = "Ranga może mieć maksymalnie 40 znaków.")]
    [Display(Name = "Ranga")]
    public string Rank { get; set; } = "Nowicjusz";

    [Display(Name = "Konto aktywne")]
    public bool IsActive { get; set; } = true;
}
