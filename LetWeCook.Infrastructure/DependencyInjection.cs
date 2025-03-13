using LetWeCook.Application.Interfaces;
using LetWeCook.Application.Services;
using LetWeCook.Domain.Events;
using LetWeCook.Infrastructure.Persistence;
using LetWeCook.Infrastructure.Repositories;
using LetWeCook.Infrastructure.Services;
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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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

        // Register Application Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();

        // Register Infrastructure Services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDietaryPreferenceRepository, DietaryPreferenceRepository>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IHttpContextService, HttpContextService>();

        return services;
    }
}