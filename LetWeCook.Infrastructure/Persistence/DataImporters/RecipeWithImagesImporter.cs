using System.Text.Json;
using System.Text.Json.Serialization;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LetWeCook.Infrastructure.Persistence.DataImporters;

public class RecipeWithImagesImporter
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRecipeService _recipeService;

    public RecipeWithImagesImporter(
        IIngredientRepository ingredientRepository,
        IRecipeRepository recipeRepository,
        UserManager<ApplicationUser> userManager,
        IRecipeService recipeService)
    {
        _ingredientRepository = ingredientRepository;
        _recipeRepository = recipeRepository;
        _userManager = userManager;
        _recipeService = recipeService;
    }

    public async Task ImportRecipesAsync(CancellationToken cancellationToken)
    {
        string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        string filePath = Path.Combine(projectRoot, "LetWeCook.Infrastructure", "Persistence", "DataImporters", "recipes-with-images.json");

        Console.WriteLine($"📂 Importing recipes with images from file: {filePath}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"❌ File not found: {filePath}");
            return;
        }

        try
        {
            string jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            var recipes = JsonSerializer.Deserialize<List<RecipeWithImages>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (recipes == null || !recipes.Any())
            {
                Console.WriteLine("⚠️ No recipes found in the JSON file.");
                return;
            }

            var users = await _userManager.Users.ToListAsync(cancellationToken);
            var random = new Random();

            foreach (var recipe in recipes)
            {
                Console.WriteLine($"🔹 Recipe: {recipe.Name}");

                bool exists = await _recipeRepository.CheckExistByNameAsync(recipe.Name, cancellationToken);
                if (exists)
                {
                    Console.WriteLine($"⚠️ Skipping '{recipe.Name}' - already exists.");
                    continue;
                }

                // Deduplicate ingredients by name (case-insensitive)
                var distinctIngredients = recipe.Ingredients
                    .GroupBy(i => i.Name.Trim().ToLowerInvariant())
                    .Select(g => g.First())
                    .ToList();

                // Then build the ingredient dictionary only for distinct ingredients
                var ingredientDict = new Dictionary<string, Guid>();
                foreach (var ing in distinctIngredients)
                {
                    var dbIng = await _ingredientRepository.GetByNameAsync(ing.Name, cancellationToken);
                    if (dbIng != null)
                        ingredientDict[ing.Name] = dbIng.Id;
                    else
                        Console.WriteLine($"❌ Ingredient '{ing.Name}' not found.");
                }

                // Build DTOs using distinctIngredients, filtering only those found in ingredientDict
                var ingredientDtos = distinctIngredients
                    .Where(i => ingredientDict.ContainsKey(i.Name))
                    .Select(i => new CreateIngredientDto
                    {
                        Id = ingredientDict[i.Name],
                        Quantity = i.Quantity,
                        Unit = i.Unit
                    })
                    .ToList();

                var steps = recipe.Instructions.Select((i, index) => new CreateStepDto
                {
                    Title = $"Step {index + 1}",
                    Description = i.Step,
                    MediaUrls = string.IsNullOrWhiteSpace(i.Image) ? new List<string>() : new List<string> { i.Image },
                    Order = index + 1
                }).ToList();

                var dto = new CreateRecipeRequestDto
                {
                    Name = recipe.Name,
                    Description = recipe.Description,
                    Servings = recipe.Servings,
                    PrepareTime = recipe.PrepTime,
                    CookTime = recipe.CookTime,
                    Difficulty = recipe.Difficulty,
                    MealCategory = recipe.MealCategory,
                    Tags = recipe.Tags ?? new List<string>(),
                    Ingredients = ingredientDtos,
                    Steps = steps,
                    CoverImage = recipe.CoverImageUrl
                };

                var user = users[random.Next(users.Count)];

                try
                {
                    Console.WriteLine($"⚡ Creating recipe '{recipe.Name}'...");
                    // var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
                    // {
                    //     WriteIndented = true,
                    //     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    // });
                    // Console.WriteLine($"📝 DTO for '{recipe.Name}':\n{json}");
                    await _recipeService.CreateRecipeForSeedAsync(user.Id, dto, cancellationToken);
                    Console.WriteLine($"✅ Successfully added recipe: {recipe.Name}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error saving '{recipe.Name}': {ex.Message}");
                }
            }

            Console.WriteLine("🎉 Import finished!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to import from file: {filePath}");
            Console.WriteLine($"📝 Exception: {ex.Message}");
        }
    }

    private class RecipeWithImages
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
        [JsonPropertyName("servings")] public int Servings { get; set; }
        [JsonPropertyName("prepTime")] public int PrepTime { get; set; }
        [JsonPropertyName("cookTime")] public int CookTime { get; set; }
        [JsonPropertyName("difficulty")] public string Difficulty { get; set; } = string.Empty;
        [JsonPropertyName("mealCategory")] public string MealCategory { get; set; } = string.Empty;
        [JsonPropertyName("coverImageUrl")] public string CoverImageUrl { get; set; } = string.Empty;
        [JsonPropertyName("tags")] public List<string>? Tags { get; set; }
        [JsonPropertyName("ingredients")] public List<Ingredient> Ingredients { get; set; } = new();
        [JsonPropertyName("instructions")] public List<InstructionWithImage> Instructions { get; set; } = new();
    }

    private class Ingredient
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("quantity")] public float Quantity { get; set; }
        [JsonPropertyName("unit")] public string Unit { get; set; } = string.Empty;
    }

    private class InstructionWithImage
    {
        [JsonPropertyName("step")] public string Step { get; set; } = string.Empty;
        [JsonPropertyName("image")] public string? Image { get; set; }
    }
}
