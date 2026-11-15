namespace Forum_Dyskuyjne.Models.Domain;

/// <summary>Kategoria grupująca fora. Forum należy do dokładnie jednej kategorii.</summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Position { get; set; }
}
