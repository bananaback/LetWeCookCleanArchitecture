namespace LetWeCook.Web.Areas.Cooking.Models.Requests;

public class RecipeQueryRequest
{
    public NameSearchOption NameSearch { get; set; } = new();
    public List<string> Difficulties { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public RangeFilter Servings { get; set; } = new();
    public RangeFilter PrepareTime { get; set; } = new();
    public RangeFilter CookTime { get; set; } = new();
    public RangeFilter Rating { get; set; } = new();
    public RangeFilter Views { get; set; } = new();
    public DateRangeFilter CreatedAt { get; set; } = new();
    public DateRangeFilter UpdatedAt { get; set; } = new();
    public string? CreatedByUsername { get; set; }
    public List<SortOption> SortOptions { get; set; } = new();
    public int ItemsPerPage { get; set; } = 10;
    public int Page { get; set; } = 1;
}

public class NameSearchOption
{
    public string Name { get; set; } = string.Empty;
    public string TextMatch { get; set; } = "exact"; // e.g., "exact" or "contains"
}

public class RangeFilter
{
    public int Min { get; set; }
    public int Max { get; set; }
}

public class DateRangeFilter
{
    public DateTime? Min { get; set; }
    public DateTime? Max { get; set; }
}

public class SortOption
{
    public string Criteria { get; set; } = string.Empty; // e.g., "name", "rating"
    public string Direction { get; set; } = "ascending"; // or "descending"
}