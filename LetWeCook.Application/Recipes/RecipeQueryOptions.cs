

namespace LetWeCook.Application.Recipes;

public class RecipeQueryOptions
{
    // 🔎 Text Filters
    public string Name { get; set; } = string.Empty;
    public string NameMatchMode { get; set; } = string.Empty;

    // ✅ Exact Value Filters
    public List<string> Difficulties { get; set; } = new();

    public List<string> MealCategories { get; set; } = new();

    // ✅ Numeric Range Filters
    public int MinServings { get; set; } = 0;
    public int MaxServings { get; set; } = 500;

    public int MinPrepareTime { get; set; } = 0;
    public int MaxPrepareTime { get; set; } = 300;

    public int MinCookTime { get; set; } = 0;
    public int MaxCookTime { get; set; } = 300;

    public double MinAverageRating { get; set; } = 0.0;
    public double MaxAverageRating { get; set; } = 5.0;

    public int MinTotalViews { get; set; } = 0;
    public int MaxTotalViews { get; set; } = 10000;

    // ✅ Date Range Filters
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public DateTime? UpdatedFrom { get; set; }
    public DateTime? UpdatedTo { get; set; }

    // ✅ Tag Filter
    public List<string> Tags { get; set; } = new();

    // ✅ Creator Filter
    public string CreatedByUsername { get; set; } = string.Empty;

    // ↕️ Sorting
    public List<SortOption> SortOptions { get; set; } = new();

    // 📄 Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SortOption
{
    public string Criteria { get; set; } = string.Empty; // e.g., "name", "rating"
    public string Direction { get; set; } = "ascending"; // or "descending"
}

