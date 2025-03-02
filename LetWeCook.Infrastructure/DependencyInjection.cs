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

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Identity/Account/Login";
               options.LogoutPath = "/Identity/Account/Logout";
               options.AccessDeniedPath = "/Identity/Account/AccessDenied";
           })
           .AddGoogle(googleOptions =>
           {
               googleOptions.ClientId = authenticationConfiguration.Google.ClientId;
               googleOptions.ClientSecret = authenticationConfiguration.Google.ClientSecret;
               googleOptions.Scope.Add("profile"); // For avatar picture

               // Add picture claim during ticket creation
               googleOptions.Events.OnCreatingTicket = context =>
               {
                   var picture = context.User.GetProperty("picture").GetString();
                   if (picture != null)
                   {
                       context.Identity?.AddClaim(new Claim("picture", picture));
                   }
                   return Task.CompletedTask;
               };

               // Force account selection
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
               facebookOptions.Fields.Add("picture"); // Request picture field

               // Add picture claim during ticket creation
               facebookOptions.Events.OnCreatingTicket = context =>
               {
                   var picture = context.User.GetProperty("picture")?.GetProperty("data")?.GetProperty("url")?.ToString();
                   if (picture != null)
                   {
                       context.Identity?.AddClaim(new Claim("picture", picture));
                   }
                   return Task.CompletedTask;
               };

               // Force re-authentication
               facebookOptions.Events.OnRedirectToAuthorizationEndpoint = context =>
               {
                   context.Response.Redirect(context.RedirectUri + "&auth_type=reauthenticate");
                   return Task.CompletedTask;
               };
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
        services.AddScoped<IHttpContextService, HttpContextService>();

        return services;
    }
}