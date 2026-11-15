using System.ComponentModel.DataAnnotations;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Lista ogłoszeń + formularz (admin).</summary>
public class AnnouncementsAdminViewModel
{
    public IReadOnlyList<Forum_Dyskuyjne.Models.Domain.Announcement> Announcements { get; set; }
        = Array.Empty<Forum_Dyskuyjne.Models.Domain.Announcement>();
    public AnnouncementFormViewModel Form { get; set; } = new();
}

public class AnnouncementFormViewModel
{
    [Required(ErrorMessage = "Podaj tytuł ogłoszenia.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Tytuł musi mieć od 3 do 120 znaków.")]
    [Display(Name = "Tytuł")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj treść ogłoszenia.")]
    [MinLength(5, ErrorMessage = "Treść musi mieć co najmniej 5 znaków.")]
    [StringLength(2000, ErrorMessage = "Treść może mieć maksymalnie 2000 znaków.")]
    [Display(Name = "Treść")]
    public string Content { get; set; } = string.Empty;
}
