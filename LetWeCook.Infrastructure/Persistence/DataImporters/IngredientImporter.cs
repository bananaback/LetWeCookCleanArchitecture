using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LetWeCook.Application.DTOs.Ingredient;
using LetWeCook.Application.Interfaces;

public class IngredientImporter
{
    private readonly IIngredientService _ingredientService;
    private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
    private readonly IIngredientRepository _ingredientRepository;

    public IngredientImporter(IIngredientService ingredientService, IIngredientCategoryRepository ingredientCategoryRepository, IIngredientRepository ingredientRepository)
    {
        _ingredientService = ingredientService;
        _ingredientCategoryRepository = ingredientCategoryRepository;
        _ingredientRepository = ingredientRepository;
    }

    public async Task ImportIngredientsAsync(string filePath, CancellationToken cancellationToken)
    {
        Console.WriteLine($"📂 Importing ingredients from file: {filePath}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"❌ File not found: {filePath}");
            return;
        }

        try
        {
            string jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            Console.WriteLine("✅ Successfully read JSON file.");

            var ingredientList = JsonSerializer.Deserialize<IngredientJsonRoot>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (ingredientList?.Ingredients == null || !ingredientList.Ingredients.Any())
            {
                Console.WriteLine("⚠️ No ingredients found in the JSON file.");
                return;
            }

            Console.WriteLine($"📊 Found {ingredientList.Ingredients.Count} ingredients to process.");

            foreach (var ingredient in ingredientList.Ingredients)
            {
                Console.WriteLine($"🔹 Processing ingredient: {ingredient.Name}");

                // ✅ Check if ingredient already exists using _ingredientRepository
                bool ingredientExists = await _ingredientRepository.CheckExistByNameAsync(ingredient.Name, cancellationToken);
                if (ingredientExists)
                {
                    Console.WriteLine($"⚠️ Skipping '{ingredient.Name}' - Already exists in the database.");
                    continue;
                }

                Guid categoryId;
                try
                {
                    categoryId = await GetCategoryIdByName(ingredient.CategoryName);
                    Console.WriteLine($"🔍 Resolved Category '{ingredient.CategoryName}' to ID: {categoryId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to resolve category '{ingredient.CategoryName}': {ex.Message}");
                    continue;
                }

                var request = new CreateIngredientRequestDto
                {
                    Name = ingredient.Name,
                    Description = ingredient.Description,
                    CategoryId = categoryId,
                    CoverImage = ingredient.CoverImageUrl,
                    ExpirationDays = ingredient.ExpirationDays,
                    NutritionValues = new NutritionValuesRequestDto
                    {
                        Calories = ingredient.Calories,
                        Protein = ingredient.Protein,
                        Carbohydrates = ingredient.Carbohydrates,
                        Fats = ingredient.Fats,
                        Sugars = ingredient.Sugars,
                        Fiber = ingredient.Fiber,
                        Sodium = ingredient.Sodium
                    },
                    DietaryInfo = new DietaryInfoRequestDto
                    {
                        IsVegetarian = ingredient.IsVegetarian,
                        IsVegan = ingredient.IsVegan,
                        IsGlutenFree = ingredient.IsGlutenFree,
                        IsPescatarian = ingredient.IsPescatarian
                    },
                    Details = ingredient.Details.Select(detail => new DetailRequestDto
                    {
                        Title = detail.Title,
                        Description = detail.Description,
                        MediaUrls = detail.MediaUrls,
                        Order = detail.Order
                    }).ToList()
                };

                try
                {
                    Console.WriteLine($"⚡ Adding ingredient '{ingredient.Name}'...");
                    // await _ingredientService.CreateIngredientAsync(null, request, cancellationToken);
                    // remember to add fixed user id here
                    Console.WriteLine($"✅ Successfully added ingredient: {ingredient.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to add ingredient '{ingredient.Name}': {ex}");
                }
            }

            Console.WriteLine("🎉 Ingredient import process completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Critical error during import: {ex}");
        }
    }


    private async Task<Guid> GetCategoryIdByName(string categoryName)
    {
        var category = await _ingredientCategoryRepository.GetByNameAsync(categoryName);

        if (category == null)
        {
            throw new Exception($"Category with name {categoryName} not found.");
        }

        return category.Id;
    }
}

// Models for JSON Deserialization
public class IngredientJsonRoot
{
    public List<IngredientJsonDto> Ingredients { get; set; } = new();
}

public class IngredientJsonDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public float Calories { get; set; }
    public float Protein { get; set; }
    public float Carbohydrates { get; set; }
    public float Fats { get; set; }
    public float Sugars { get; set; }
    public float Fiber { get; set; }
    public float Sodium { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsPescatarian { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public int ExpirationDays { get; set; }
    public List<DetailJsonDto> Details { get; set; } = new();
}

public class DetailJsonDto
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
}
