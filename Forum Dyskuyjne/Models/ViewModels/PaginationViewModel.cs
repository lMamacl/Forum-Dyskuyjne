namespace Forum_Dyskuyjne.Models.ViewModels;

/// <summary>
/// Model wspólnego widoku paginacji. RouteValues trzyma parametry trasy, które mają przetrwać
/// zmianę strony (np. forumId, id, q) — parametr „page” doklejany jest automatycznie.
/// </summary>
public class PaginationViewModel
{
    public string Action { get; set; } = "Index";
    public IDictionary<string, string> RouteValues { get; set; } =
        new Dictionary<string, string>();
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;

    public IDictionary<string, string> RouteValuesWithPage(int page)
    {
        var data = new Dictionary<string, string>(RouteValues) { ["page"] = page.ToString() };
        return data;
    }
}
