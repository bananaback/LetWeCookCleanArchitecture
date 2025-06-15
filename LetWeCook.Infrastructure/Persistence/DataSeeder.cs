using System.Text.Json;
using LetWeCook.Application.Dtos;
using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Domain.Entities;
using LetWeCook.Domain.Enums;
using LetWeCook.Infrastructure.Persistence.DataImporters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LetWeCook.Infrastructure.Persistence;

public class DataSeeder
{
    public static async Task SeedIngredientsAsync(IServiceProvider services, string jsonFilePath, CancellationToken cancellationToken)
    {
        var ingredientService = services.GetRequiredService<IIngredientService>();
        var ingredientCategoryRepository = services.GetRequiredService<IIngredientCategoryRepository>();
        var ingredientRepository = services.GetRequiredService<IIngredientRepository>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        var importer = new IngredientImporter(ingredientService, ingredientCategoryRepository, ingredientRepository, userManager);

        try
        {
            await importer.ImportIngredientsAsync(jsonFilePath, cancellationToken);
            logger.LogInformation("Successfully imported ingredients from JSON.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while importing ingredients.");
        }
    }

    public static async Task SeedRecipesAsync(IServiceProvider services, string jsonFilePath, CancellationToken cancellationToken)
    {
        var recipeService = services.GetRequiredService<IRecipeService>();
        var recipeRepository = services.GetRequiredService<IRecipeRepository>();
        var ingredientRepository = services.GetRequiredService<IIngredientRepository>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        var importer = new RecipeImporter(
            ingredientRepository,
            recipeRepository,
            userManager,
            recipeService);

        try
        {
            await importer.ImportRecipesAsync(jsonFilePath, cancellationToken);
            logger.LogInformation("Successfully imported recipes from JSON.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while importing recipes.");
        }
    }

    public static async Task SeedRecipesWithImagesAsync(IServiceProvider services, string jsonFilePath, CancellationToken cancellationToken)
    {
        var recipeService = services.GetRequiredService<IRecipeService>();
        var recipeRepository = services.GetRequiredService<IRecipeRepository>();
        var ingredientRepository = services.GetRequiredService<IIngredientRepository>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        var importer = new RecipeWithImagesImporter(
            ingredientRepository,
            recipeRepository,
            userManager,
            recipeService);

        try
        {
            await importer.ImportRecipesAsync(jsonFilePath, cancellationToken);
            logger.LogInformation("Successfully imported recipes from JSON.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while importing recipes.");
        }
    }


    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var userRepository = services.GetRequiredService<IUserRepository>(); // For SiteUser
        var unitOfWork = services.GetRequiredService<IUnitOfWork>(); // For committing changes
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        // Seed Roles
        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        {
            string roleName = role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                try
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Successfully created role: {RoleName}", roleName);
                    }
                    else
                    {
                        logger.LogError("Failed to create role: {RoleName}. Errors: {Errors}",
                            roleName,
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while creating role: {RoleName}", roleName);
                }
            }
            else
            {
                logger.LogInformation("Role already exists: {RoleName}", roleName);
            }
        }

        // Seed Default Admin User
        const string adminUsername = "admin";
        const string adminPassword = "Admin100%"; // Weak for testing; strengthen in production
        const string adminEmail = "admin@letwecook.com";

        var existingAdmin = await userManager.FindByNameAsync(adminUsername);
        if (existingAdmin == null)
        {
            // Create SiteUser first
            var siteUser = new SiteUser(adminEmail, false); // Matches your RegisterAsync pattern
            try
            {
                await userRepository.AddAsync(siteUser);

                // Create ApplicationUser referencing SiteUser.Id
                var adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    SiteUserId = siteUser.Id,
                    EmailConfirmed = true // Admin doesn't need verification
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, UserRole.Admin.ToString());
                    if (roleResult.Succeeded)
                    {
                        await unitOfWork.CommitAsync(); // Commit SiteUser and ApplicationUser together
                        logger.LogInformation("Successfully created and assigned 'Admin' role to user: {Username}", adminUsername);
                    }
                    else
                    {
                        logger.LogError("Failed to assign 'Admin' role to {Username}. Errors: {Errors}",
                            adminUsername,
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogError("Failed to create admin user: {Username}. Errors: {Errors}",
                        adminUsername,
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding admin user: {Username}", adminUsername);
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists: {Username}", adminUsername);
        }
    }

    public static async Task SeedUsersAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        var userService = services.GetRequiredService<IUserService>();

        var seedUserDTOs = new List<SeedUserDTO>();
        for (int i = 1; i <= 100; i++)
        {
            seedUserDTOs.Add(new SeedUserDTO
            {
                Email = $"user{i}@example.com",
                Password = $"User@{i}00%",
                Username = $"user{i}",
                IsAdmin = false
            });
        }

        await userService.SeedUsersAsync(seedUserDTOs);

        Console.WriteLine($"[SEED] {seedUserDTOs.Count} users seeded.");
    }

    public static async Task SeedUserProfiles(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var userService = services.GetRequiredService<IUserService>();

        await userService.SeedUserProfiles(cancellationToken);

    }

    public static async Task SeedRecipeRatingsAsync(IServiceProvider services, int amount, CancellationToken cancellationToken = default)
    {
        var recipeRatingService = services.GetRequiredService<IRecipeRatingService>();
        await recipeRatingService.SeedRecipeRatingsAsync(amount, cancellationToken);
    }

    public static async Task SeedRecipeDonationsAsync(IServiceProvider services, int amount, CancellationToken cancellationToken = default)
    {
        var donationService = services.GetRequiredService<IDonationService>();
        await donationService.SeedRecipeDonationsAsync(amount, cancellationToken);
    }

    public static async Task SeedSuggestionFeedbacksAsync(IServiceProvider services, int amount, CancellationToken cancellationToken = default)
    {
        string infrastructureProjectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LetWeCook.Infrastructure"));
        string jsonFilePath = Path.Combine(infrastructureProjectPath, "Persistence", "DataImporters", "preferences-recipes.json");

        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException("The preferences-recipes.json file was not found.", jsonFilePath);

        string jsonContent = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
        var dietaryRecipeMap = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonContent)
            ?? throw new InvalidOperationException("Failed to deserialize preferences-recipes.json.");

        var userRepository = services.GetRequiredService<IUserRepository>();
        var recipeRepository = services.GetRequiredService<IRecipeRepository>();
        var suggestionFeedbackRepository = services.GetRequiredService<ISuggestionFeedbackRepository>();
        var suggestionFeedbackService = services.GetRequiredService<IRecipeSuggestionService>();

        var users = await userRepository.GetAllWithProfileAsync(cancellationToken);
        if (users == null || !users.Any())
            throw new InvalidOperationException("No users found in the database.");

        // Get all recipes once for dislike random picks
        var allRecipes = await recipeRepository.GetAllAsync(cancellationToken);
        if (allRecipes == null || !allRecipes.Any())
            throw new InvalidOperationException("No recipes found in the database.");

        var random = new Random();

        // check existing suggestion feedbacks number by get all then count
        var existingFeedbacks = await suggestionFeedbackRepository.GetAllAsync(cancellationToken);
        var existingFeedbackCount = existingFeedbacks?.Count() ?? 0;
        var remainAmount = amount - existingFeedbackCount;
        if (remainAmount <= 0)
        {
            Console.WriteLine("No new suggestion feedbacks to seed, as the existing count meets or exceeds the requested amount.");
            return;
        }
        while (remainAmount > 0)
        {
            var randomUser = users[random.Next(users.Count)];
            if (randomUser.Profile == null)
                continue;

            var dietaryPreferences = randomUser.Profile.DietaryPreferences?.Select(dp => dp.Name).ToList();
            if (dietaryPreferences == null || !dietaryPreferences.Any())
                continue;

            // For example, seed 70% likes, 30% dislikes (adjust ratio as needed)
            double likeRatio = 0.7;

            if (random.NextDouble() <= likeRatio)
            {
                // Seed LIKE feedback using dietary preferences recipes
                foreach (var preference in dietaryPreferences)
                {
                    if (!dietaryRecipeMap.TryGetValue(preference, out var recipes) || !recipes.Any())
                        continue;

                    var randomRecipeName = recipes[random.Next(recipes.Count)];
                    var randomRecipe = await recipeRepository.GetByNameAsync(randomRecipeName, cancellationToken);
                    if (randomRecipe == null)
                        continue;

                    await suggestionFeedbackService.ProcessFeedbackAsync(randomRecipe.Id, true, randomUser.Id, cancellationToken);
                    remainAmount--;
                    if (remainAmount <= 0)
                        break;
                }
            }
            else
            {
                // Seed DISLIKE feedback using random recipe from all recipes
                var randomRecipe = allRecipes[random.Next(allRecipes.Count)];
                await suggestionFeedbackService.ProcessFeedbackAsync(randomRecipe.Id, false, randomUser.Id, cancellationToken);
                remainAmount--;
            }
        }
    }

    private static readonly Dictionary<string, string> PreferenceNameMap = new Dictionary<string, string>
    {
        { "Pref_Vegetarian", "Vegetarian" },
        { "Pref_Vegan", "Vegan" },
        { "Pref_GlutenFree", "GlutenFree" },
        { "Pref_Pescatarian", "Pescatarian" },
        { "Pref_LowCalorie", "LowCalorie" },
        { "Pref_HighProtein", "HighProtein" },
        { "Pref_LowCarb", "LowCarb" },
        { "Pref_LowFat", "LowFat" },
        { "Pref_LowSugar", "LowSugar" },
        { "Pref_HighFiber", "HighFiber" },
        { "Pref_LowSodium", "LowSodium" }
    };

    public static async Task SeedUserInteractionsAsync(IServiceProvider services, int amount, CancellationToken cancellationToken = default)
    {
        var userRepository = services.GetRequiredService<IUserRepository>();
        var recipeRepository = services.GetRequiredService<IRecipeRepository>();
        var userInteractionRepository = services.GetRequiredService<IUserInteractionRepository>();
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        // Define the path to the JSON file
        string infrastructureProjectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LetWeCook.Infrastructure"));
        string jsonFilePath = Path.Combine(infrastructureProjectPath, "Persistence", "DataImporters", "ultimate-preferences-recipes.json");

        try
        {
            // Validate JSON file existence
            if (!File.Exists(jsonFilePath))
            {
                logger.LogError("The ultimate-preferences-recipes.json file was not found at {Path}", jsonFilePath);
                throw new FileNotFoundException("The ultimate-preferences-recipes.json file was not found.", jsonFilePath);
            }

            // Read and deserialize JSON
            string jsonContent = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
            var dietaryRecipeMap = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(jsonContent)
                ?? throw new InvalidOperationException("Failed to deserialize ultimate-preferences-recipes.json.");

            // Fetch users with profiles
            var users = await userRepository.GetAllWithProfileAsync(cancellationToken);
            if (users == null || !users.Any())
            {
                logger.LogError("No users with profiles found in the database.");
                throw new InvalidOperationException("No users found in the database.");
            }

            // Fetch all recipes
            var allRecipes = await recipeRepository.GetAllAsync(cancellationToken);
            if (allRecipes == null || !allRecipes.Any())
            {
                logger.LogError("No recipes found in the database.");
                throw new InvalidOperationException("No recipes found in the database.");
            }

            // Check existing interactions
            var existingInteractions = await userInteractionRepository.GetAllAsync(cancellationToken);
            var existingInteractionCount = existingInteractions?.Count() ?? 0;
            var remainAmount = amount - existingInteractionCount;
            if (remainAmount <= 0)
            {
                logger.LogInformation("No new user interactions to seed, as the existing count ({ExistingCount}) meets or exceeds the requested amount ({RequestedAmount}).", existingInteractionCount, amount);
                Console.WriteLine($"No new user interactions to seed, as the existing count ({existingInteractionCount}) meets or exceeds the requested amount ({amount}).");
                return;
            }

            var random = new Random();
            var interactionsToAdd = new List<UserInteraction>();
            // Define interaction type distribution (approximate percentages)
            var interactionTypes = new[] { "like", "dislike", "view", "rating", "comment", "donate" };
            var weights = new[] { 0.30, 0.10, 0.30, 0.15, 0.10, 0.05 }; // Sum to 1.0
            double likeRatio = 0.75; // 75% of like/dislike interactions are likes

            while (remainAmount > 0)
            {
                // Select a random user
                var randomUser = users[random.Next(users.Count)];
                if (randomUser.Profile == null || randomUser.Profile.DietaryPreferences == null || !randomUser.Profile.DietaryPreferences.Any())
                {
                    continue; // Skip users without profiles or dietary preferences
                }

                var dietaryPreferences = randomUser.Profile.DietaryPreferences.Select(dp => dp.Name).ToList();
                Domain.Aggregates.Recipe? randomRecipe = null;
                bool isLikeInteraction = false;

                // Determine interaction type based on weights
                string interactionType = ChooseWeightedRandom(interactionTypes, weights, random);

                if (interactionType == "like" || interactionType == "dislike")
                {
                    isLikeInteraction = interactionType == "like" || random.NextDouble() <= likeRatio;
                    interactionType = isLikeInteraction ? "like" : "dislike";

                    // Select recipe based on like/dislike
                    if (isLikeInteraction)
                    {
                        // Choose a recipe from the "like" list for a random preference
                        var dbPref = dietaryPreferences[random.Next(dietaryPreferences.Count)];
                        var jsonPref = PreferenceNameMap.FirstOrDefault(kvp => kvp.Value == dbPref).Key;
                        if (jsonPref != null && dietaryRecipeMap.TryGetValue(jsonPref, out var recipeMap) && recipeMap.TryGetValue("like", out var likeRecipes) && likeRecipes.Any())
                        {
                            var recipeName = likeRecipes[random.Next(likeRecipes.Count)];
                            randomRecipe = await recipeRepository.GetByNameAsync(recipeName, cancellationToken);
                        }
                    }
                    else
                    {
                        // Choose a recipe from the "dislike" list or random recipe
                        var dbPref = dietaryPreferences[random.Next(dietaryPreferences.Count)];
                        var jsonPref = PreferenceNameMap.FirstOrDefault(kvp => kvp.Value == dbPref).Key;
                        if (jsonPref != null && dietaryRecipeMap.TryGetValue(jsonPref, out var recipeMap) && recipeMap.TryGetValue("dislike", out var dislikeRecipes) && dislikeRecipes.Any())
                        {
                            var recipeName = dislikeRecipes[random.Next(dislikeRecipes.Count)];
                            randomRecipe = await recipeRepository.GetByNameAsync(recipeName, cancellationToken);
                        }
                    }

                    // Fallback to random recipe if none selected
                    randomRecipe ??= allRecipes[random.Next(allRecipes.Count)];
                }
                else
                {
                    // For other interactions, select a random recipe
                    randomRecipe = allRecipes[random.Next(allRecipes.Count)];
                }

                if (randomRecipe == null)
                {
                    logger.LogWarning("No suitable recipe found for interaction.");
                    continue;
                }

                float eventValue = interactionType switch
                {
                    "like" => 1.0f,
                    "dislike" => 1.0f,
                    "view" => 1.0f,
                    "rating" => random.Next(1, 6), // 1 to 5
                    "donate" => (float)Math.Round(random.NextDouble() * 50 + 5, 2), // $5 to $55
                    "comment" => random.Next(10, 101), // Random length between 10 and 100
                    _ => 0.0f
                };

                interactionsToAdd.Add(new UserInteraction(
                    userId: randomUser.Id,
                    recipeId: randomRecipe.Id,
                    interactionType: interactionType,
                    eventValue: eventValue
                ));

                remainAmount--;

                // Perform bulk insert in batches of 1000
                if (interactionsToAdd.Count >= 1600 || remainAmount <= 0)
                {
                    try
                    {
                        await userInteractionRepository.AddRangeAsync(interactionsToAdd, cancellationToken);
                        await unitOfWork.CommitAsync(cancellationToken); // Commit the batch
                        logger.LogInformation("Successfully seeded {Count} user interactions.", interactionsToAdd.Count);
                        interactionsToAdd.Clear();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while seeding user interactions.");
                    }
                }
            }

            // Insert remaining interactions
            if (interactionsToAdd.Any())
            {
                try
                {
                    await userInteractionRepository.AddRangeAsync(interactionsToAdd, cancellationToken);
                    await unitOfWork.CommitAsync(cancellationToken); // Commit the last batch
                    logger.LogInformation("Successfully seeded {Count} user interactions.", interactionsToAdd.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding user interactions.");
                }
            }

            logger.LogInformation("Successfully completed seeding {TotalAmount} user interactions.", amount - remainAmount);
            Console.WriteLine($"[SEED] {amount - remainAmount} user interactions seeded.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding user interactions.");
            throw;
        }
    }

    // Helper method for weighted random selection
    private static string ChooseWeightedRandom(string[] items, double[] weights, Random random)
    {
        double totalWeight = weights.Sum();
        double randomValue = random.NextDouble() * totalWeight;
        double cumulativeWeight = 0;

        for (int i = 0; i < items.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return items[i];
            }
        }

        return items[^1]; // Fallback to last item
    }
}