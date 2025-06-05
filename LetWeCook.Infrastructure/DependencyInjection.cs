using LetWeCook.Application.Interfaces;
using LetWeCook.Application.Services;
using LetWeCook.Domain.Events;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Infrastructure.Repositories;
using LetWeCook.Infrastructure.Services;
using LetWeCook.Infrastructure.Services.Background;
using LetWeCook.Infrastructure.Services.EventHandlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetWeCook.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        // Add DbContext
        services.AddDbContext<LetWeCookDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging(); // helpful during dev

        });

        // Add Identity with configuration
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddEntityFrameworkStores<LetWeCookDbContext>()
            .AddDefaultTokenProviders();

        // Register Email Services
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IEmailService, EmailService>();

        // Register Domain Event Handlers
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<UserRegisteredEvent>, UserRegisteredLoggingHandler>();
        services.AddScoped<INonBlockingDomainEventHandler<UserRegisteredEvent>, UserRegisteredEmailHandler>();
        services.AddScoped<INonBlockingDomainEventHandler<UserRequestedEmailEvent>, UserRequestedEmailHandler>();
        services.AddScoped<INonBlockingDomainEventHandler<UserRequestedPasswordResetEvent>, UserRequestedPasswordResetEventHandler>();
        services.AddScoped<INonBlockingDomainEventHandler<RecipeSnapshotRequestedEvent>, RecipeSnapshotRequestedHandler>();
        // Register Application Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IPaymentService, PayPalPaymentService>();
        services.AddScoped<IDonationService, DonationService>();
        services.AddScoped<IRecipeRatingService, RecipeRatingService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRecipeSuggestionService, RecipeSuggestionService>();
        services.AddScoped<ICollectionService, RecipeCollectionService>();
        services.AddScoped<IRecipeSnapshotService, RecipeSnapshotService>();
        services.AddScoped<IPredictionService, PredictionService>();

        // Register Infrastructure Services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDietaryPreferenceRepository, DietaryPreferenceRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IIngredientCategoryRepository, IngredientCategoryRepository>();
        services.AddScoped<IMediaUrlRepository, MediaUrlRepository>();
        services.AddScoped<IDetailRepository, DetailRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IUserRequestRepository, UserRequestRepository>();
        services.AddScoped<IUserRequestRepository, UserRequestRepository>();
        services.AddScoped<IRecipeTagRepository, RecipeTagRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IDonationRepository, DonationRepository>();
        services.AddScoped<IRecipeRatingRepository, RecipeRatingRepository>();
        services.AddScoped<ISuggestionFeedbackRepository, SuggestionFeedbackRepository>();
        services.AddScoped<IRecipeCollectionRepository, RecipeCollectionRepository>();
        services.AddScoped<IRecipeCollectionItemRepository, RecipeCollectionItemRepository>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IHttpContextService, HttpContextService>();

        services.AddHostedService<RecipeExportService>();
        services.AddHostedService<HomepageService>();
        // TEMPORARY comment
        services.AddHostedService<TrainingDataService>();

        services.AddHttpClient();


        return services;
    }
}