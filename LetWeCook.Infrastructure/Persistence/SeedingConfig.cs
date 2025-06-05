namespace LetWeCook.Infrastructure.Persistence;

public class SeedingConfig
{
    public bool SeedRolesAndAdmin { get; set; }
    public bool SeedUsers { get; set; }
    public bool SeedUserProfiles { get; set; }
    public bool SeedIngredients { get; set; }
    public bool SeedRecipes { get; set; }
    public bool SeedRecipesWithImages { get; set; }
    public bool SeedRecipeRatings { get; set; }
    public bool SeedRecipeDonations { get; set; }
    public bool SeedSuggestionFeedbacks { get; set; }
}
