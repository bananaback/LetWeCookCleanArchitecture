using LetWeCook.Domain.Enums;
using LetWeCook.Domain.Entities; // For SiteUser
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LetWeCook.Domain.Aggregates;
using LetWeCook.Application.Interfaces;

namespace LetWeCook.Infrastructure.Persistence;

public class DataSeeder
{
    public static async Task SeedIngredientsAsync(IServiceProvider services, string jsonFilePath, CancellationToken cancellationToken)
    {
        var ingredientService = services.GetRequiredService<IIngredientService>();
        var ingredientCategoryRepository = services.GetRequiredService<IIngredientCategoryRepository>();
        var ingredientRepository = services.GetRequiredService<IIngredientRepository>();
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        var importer = new IngredientImporter(ingredientService, ingredientCategoryRepository, ingredientRepository);

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
}