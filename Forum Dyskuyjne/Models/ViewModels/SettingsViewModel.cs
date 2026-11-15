using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>
/// Ustawienia profilu. Pola_personalizacji są w etapie 1 zapisywane po stronie klienta (auth.js / cookies),
/// a w etapie 2 trafią do konta użytkownika w bazie — widok i walidacja pozostają bez zmian.
/// </summary>
public class SettingsViewModel
{
    [Required(ErrorMessage = "Podaj nazwę wyświetlaną.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 30 znaków.")]
    [Display(Name = "Nazwa wyświetlana")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj adres e-mail.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
    [Display(Name = "Adres e-mail")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Aktualne hasło")]
    public string? CurrentPassword { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Nowe hasło musi mieć co najmniej 8 znaków.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$",
        ErrorMessage = "Nowe hasło musi zawierać co najmniej jedną literę i jedną cyfrę.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nowe hasło")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Hasła nie są identyczne.")]
    [Display(Name = "Powtórz nowe hasło")]
    public string? ConfirmNewPassword { get; set; }

    [Range(5, 50, ErrorMessage = "Liczba wiadomości na stronie musi być z zakresu 5–50.")]
    [Display(Name = "Liczba wiadomości na stronie")]
    public int PageSize { get; set; } = 10;

    [Range(1, 1440, ErrorMessage = "Czas bezczynności musi być z zakresu 1–1440 minut.")]
    [Display(Name = "Wyloguj po bezczynności (minuty)")]
    public int IdleMinutes { get; set; } = 30;

    [Display(Name = "Motyw interfejsu")]
    public string Theme { get; set; } = "light";
}
