using System.Text.Json;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Infrastructure.Services.Background;

public class RecipeExportService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecipeExportService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(6);

    public RecipeExportService(IServiceProvider serviceProvider, ILogger<RecipeExportService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<LetWeCookDbContext>();
                var recipes = await db.Recipes
                    .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                    .ToListAsync(stoppingToken);

                var recipeJsonPath = "wwwroot/data/recipes.json";
                var indexJsonPath = "wwwroot/data/ingredient-index.json";

                await ExportRecipesJson(recipes, recipeJsonPath);
                await ExportIngredientIndex(recipes, indexJsonPath);

                _logger.LogInformation("✅ JSON export completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to export recipes.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ExportRecipesJson(List<Recipe> recipes, string path)
    {
        var simplified = recipes.Select(r => new
        {
            id = r.Id,
            name = r.Name,
            ingredients = r.RecipeIngredients.Select(i => i.Ingredient.Name.ToLower()).ToList()
        });

        var json = JsonSerializer.Serialize(simplified, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }

    private async Task ExportIngredientIndex(List<Recipe> recipes, string path)
    {
        var index = new Dictionary<string, HashSet<Guid>>();

        foreach (var recipe in recipes)
        {
            foreach (var ing in recipe.RecipeIngredients)
            {
                var name = ing.Ingredient.Name.ToLower();
                if (!index.ContainsKey(name))
                    index[name] = new HashSet<Guid>();
                index[name].Add(recipe.Id);
            }
        }

        // Convert HashSet to List for JSON serialization
        var simplified = index.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());

        var json = JsonSerializer.Serialize(simplified, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }


}