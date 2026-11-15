using Forum_Dyskuyjne.Models.Domain;

namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>Pulpit administracyjny: podsumowania i skróty.</summary>
public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalThreads { get; set; }
    public int TotalMessages { get; set; }
    public int TotalForums { get; set; }
    public int TotalCategories { get; set; }
    public int TotalAnnouncements { get; set; }
    public IReadOnlyList<UserAccount> RecentUsers { get; set; } = Array.Empty<UserAccount>();
}
