using LetWeCook.Infrastructure;
using LetWeCook.Infrastructure.Configurations;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Web.Areas.Identity.ViewModelValidators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});

// Register the custom validation adapter provider
builder.Services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // Prevent access to cookies via JavaScript
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are only sent over HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax; // Mitigate CSRF attacks
    options.LoginPath = "/Identity/Account/Login"; // Correct path for Identity area
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ReturnUrlParameter = "ReturnUrl";
    options.SlidingExpiration = true; // Refresh expiration time on each request
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set the cookie expiration time

});

// Bind Authentication Configuration
var authenticationConfiguration = new AuthenticationConfiguration();
builder.Configuration.GetSection("Authentications").Bind(authenticationConfiguration);
builder.Services.AddSingleton(authenticationConfiguration);

// Configure Authentication
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = authenticationConfiguration.Google.ClientId;
        googleOptions.ClientSecret = authenticationConfiguration.Google.ClientSecret;
        googleOptions.Scope.Add("profile");
        googleOptions.Events.OnCreatingTicket = (context) =>
        {
            var picture = context.User.GetProperty("picture").GetString();
            if (picture != null)
            {
                context.Identity?.AddClaim(new Claim("picture", picture));
            }
            return Task.CompletedTask;
        };
        googleOptions.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
            return Task.CompletedTask;
        };
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = authenticationConfiguration.Facebook.ClientId;
        facebookOptions.AppSecret = authenticationConfiguration.Facebook.ClientSecret;
        facebookOptions.Fields.Add("picture");
        facebookOptions.Events.OnCreatingTicket = (context) =>
        {
            var picture = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").ToString();
            if (picture != null)
            {
                context.Identity?.AddClaim(new Claim("picture", picture));
            }
            return Task.CompletedTask;
        };
        facebookOptions.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            context.Response.Redirect(context.RedirectUri + "&auth_type=reauthenticate");
            return Task.CompletedTask;
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = SeedingConfigHelper.LoadConfig();

    try
    {
        if (config.SeedRolesAndAdmin)
        {
            await DataSeeder.SeedRolesAndAdminAsync(services);
            config.SeedRolesAndAdmin = false;
        }

        if (config.SeedUsers)
        {
            await DataSeeder.SeedUsersAsync(services);
            config.SeedUsers = false;
        }

        if (config.SeedUserProfiles)
        {
            await DataSeeder.SeedUserProfiles(services);
            config.SeedUserProfiles = false;
        }

        if (config.SeedIngredients)
        {
            string path = "../LetWeCook.Infrastructure/Persistence/DataImporters/ingredients.json";
            await DataSeeder.SeedIngredientsAsync(services, path, CancellationToken.None);
            config.SeedIngredients = false;
        }

        if (config.SeedRecipes)
        {
            string path = "../LetWeCook.Infrastructure/Persistence/DataImporters/recipes.json";
            await DataSeeder.SeedRecipesAsync(services, path, CancellationToken.None);
            config.SeedRecipes = false;
        }

        if (config.SeedRecipesWithImages)
        {
            string path = "../LetWeCook.Infrastructure/Persistence/DataImporters/recipes-with-images.json";
            await DataSeeder.SeedRecipesWithImagesAsync(services, path, CancellationToken.None);
            config.SeedRecipesWithImages = false;
        }

        if (config.SeedRecipeRatings)
        {
            await DataSeeder.SeedRecipeRatingsAsync(services, 1000, CancellationToken.None);
            config.SeedRecipeRatings = false;
        }

        if (config.SeedRecipeDonations)
        {
            await DataSeeder.SeedRecipeDonationsAsync(services, 1000, CancellationToken.None);
            config.SeedRecipeDonations = false;
        }

        if (config.SeedSuggestionFeedbacks)
        {
            await DataSeeder.SeedSuggestionFeedbacksAsync(services, 5000, CancellationToken.None);
            config.SeedSuggestionFeedbacks = false;
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
    finally
    {
        SeedingConfigHelper.SaveConfig(config);
    }
}


app.Run();

namespace LetWeCook.Web
{
    public partial class Program { }
}