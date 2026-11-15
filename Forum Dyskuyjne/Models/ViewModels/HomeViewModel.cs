using System.ComponentModel.DataAnnotations;
using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Dane strony głównej: powitanie, statystyki, ogłoszenia, ostatnie wątki.</summary>
public class HomeViewModel
{
    public int TotalUsers { get; set; }
    public int TotalThreads { get; set; }
    public int TotalMessages { get; set; }
    public int TotalForums { get; set; }
    public IReadOnlyList<Announcement> Announcements { get; set; } = Array.Empty<Announcement>();
    public IReadOnlyList<ThreadListItemViewModel> RecentThreads { get; set; } = Array.Empty<ThreadListItemViewModel>();
}
