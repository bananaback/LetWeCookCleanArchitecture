using LetWeCook.Application.Interfaces;
using LetWeCook.Application.Services;
using LetWeCook.Domain.Events;
using LetWeCook.Infrastructure.Persistence;
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

        // Configure cookie-based authentication
        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

        // Register Email Services
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<UserRegisteredEvent>, UserRegisteredLoggingHandler>();
        services.AddScoped<INonBlockingDomainEventHandler<UserRegisteredEvent>, UserRegisteredEmailHandler>();
        services.AddScoped<INonBlockingDomainEventHandler<UserRequestedEmailEvent>, UserRequestedEmailHandler>();
        services.AddScoped<INonBlockingDomainEventHandler<UserRequestedPasswordResetEvent>, UserRequestedPasswordResetEventHandler>();


        // Register Application Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        // Register Infrastructure Services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}