using System.Text.Json;
using System.Text.Json.Serialization;
using LetWeCook.Application.DTOs.Recipe;
using LetWeCook.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LetWeCook.Infrastructure.Persistence.DataImporters;

public class RecipeImporter
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRecipeService _recipeService;

    public RecipeImporter(
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

    public async Task ImportRecipesAsync(string filePath, CancellationToken cancellationToken)
    {
        Console.WriteLine($"📂 Importing recipes from file: {filePath}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"❌ File not found: {filePath}");
            return;
        }

        try
        {
            string jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            Console.WriteLine("✅ Successfully read JSON file.");

            var recipeList = JsonSerializer.Deserialize<RecipeJsonRoot>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (recipeList?.Recipes == null || !recipeList.Recipes.Any())
            {
                Console.WriteLine("⚠️ No recipes found in the JSON file.");
                return;
            }

            Console.WriteLine($"📊 Found {recipeList.Recipes.Count} recipes to process.");

            foreach (var recipe in recipeList.Recipes)
            {
                Console.WriteLine($"🔹 Recipe: {recipe.Name}");
                Console.WriteLine($"   📖 Description: {recipe.Description}");
                Console.WriteLine($"   🍽️ Servings: {recipe.Servings}");
                Console.WriteLine($"   ⏱️ Prep Time: {recipe.PrepTime} mins | Cook Time: {recipe.CookTime} mins");
                Console.WriteLine($"   🎯 Difficulty: {recipe.Difficulty}");
                Console.WriteLine($"   📂 Meal Category: {recipe.MealCategory}");
                Console.WriteLine($"   🖼️ Cover Image: {recipe.CoverImageUrl}");

                // Print tags
                if (recipe.Tags != null && recipe.Tags.Any())
                {
                    Console.WriteLine($"   🏷️ Tags: {string.Join(", ", recipe.Tags)}");
                }

                // Print ingredients
                Console.WriteLine($"   📝 Ingredients:");
                foreach (var ingredient in recipe.Ingredients)
                {
                    Console.WriteLine($"      - {ingredient.Quantity} {ingredient.Unit} {ingredient.Name}");
                }

                // Print instructions
                Console.WriteLine($"   📋 Instructions:");
                int step = 1;
                foreach (var instruction in recipe.Instructions)
                {
                    Console.WriteLine($"      Step {step++}: {instruction}");
                }

                Console.WriteLine(); // Add empty line between recipes

                // ✅ Check if recipe already exists using _recipeRepository
                bool recipeExists = await _recipeRepository.CheckExistByNameAsync(recipe.Name, cancellationToken);
                if (recipeExists)
                {
                    Console.WriteLine($"⚠️ Skipping '{recipe.Name}' - Already exists in the database.");
                    continue;
                }

                // create hash map with ingredient names and their IDs
                var ingredientDict = new Dictionary<string, Guid>();
                foreach (var ingredient in recipe.Ingredients)
                {
                    var existingIngredient = await _ingredientRepository.GetByNameAsync(ingredient.Name, cancellationToken);
                    if (existingIngredient != null)
                    {
                        ingredientDict[ingredient.Name] = existingIngredient.Id;
                    }
                    else
                    {
                        Console.WriteLine($"❌ Ingredient '{ingredient.Name}' not found in the database.");
                        continue;
                    }
                }
                var ingredientDtos = recipe.Ingredients
                    .Where(ingredient => ingredientDict.ContainsKey(ingredient.Name))
                    .Select(ingredient => new CreateIngredientDto
                    {
                        Id = ingredientDict[ingredient.Name],
                        Quantity = ingredient.Quantity,
                        Unit = ingredient.Unit
                    }).ToList();

                // Map incoming CreateRecipeRequest to CreateRecipeRequestDto
                var recipeDto = new CreateRecipeRequestDto
                {
                    Name = recipe.Name,
                    Description = recipe.Description,
                    Servings = recipe.Servings,
                    PrepareTime = recipe.PrepTime,
                    CookTime = recipe.CookTime,
                    Difficulty = recipe.Difficulty,
                    MealCategory = recipe.MealCategory,
                    Tags = recipe.Tags!,
                    Ingredients = ingredientDtos,
                    Steps = recipe.Instructions.Select((instruction, i) => new CreateStepDto
                    {
                        Title = "Step " + (i + 1),
                        Description = instruction,
                        MediaUrls = new List<string>()
                    }).ToList(),
                    CoverImage = recipe.CoverImageUrl
                };

                // Check if user exists
                var user = await _userManager.FindByNameAsync("votrongtin882003@gmail.com");
                if (user == null)
                {
                    // If user is not found, just log and terminate
                    Console.WriteLine("❌ User not found. Import process terminated.");
                    return; // Terminates the method without throwing exception
                }

                try
                {
                    Console.WriteLine($"⚡ Adding recipe '{recipe.Name}'...");
                    await _recipeService.CreateRecipeForSeedAsync(user.Id, recipeDto, cancellationToken);
                    Console.WriteLine($"✅ Successfully added recipe: {recipe.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to add recipe '{recipe.Name}': {ex}");
                }
            }

            Console.WriteLine("🎉 Recipe import process completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error importing recipes from file: {filePath}");
            Console.WriteLine($"📝 Exception message: {ex.Message}");
        }
    }
}

public class RecipeJsonRoot
{
    [JsonPropertyName("recipes")]
    public List<Recipe> Recipes { get; set; } = new();
}

public class Recipe
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("servings")]
    public int Servings { get; set; }

    [JsonPropertyName("prepTime")]
    public int PrepTime { get; set; }

    [JsonPropertyName("cookTime")]
    public int CookTime { get; set; }

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonPropertyName("mealCategory")]
    public string MealCategory { get; set; } = string.Empty;

    [JsonPropertyName("coverImageUrl")]
    public string CoverImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("ingredients")]
    public List<Ingredient> Ingredients { get; set; } = new();

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = new();
}

public class Ingredient
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public float Quantity { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;
}
