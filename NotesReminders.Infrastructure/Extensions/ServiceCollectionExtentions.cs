using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;

using NotesReminders.Infrastructure.Data;
using NotesReminders.Infrastructure.Security;
using NotesReminders.Infrastructure.Repositories;
using NotesReminders.Application.Interfaces;
using NotesReminders.Domain.Entities;
using NotesReminders.Infrastructure.Notification;


namespace NotesReminders.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddNotifications();
        services.AddSecurity(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();

        return services;
    }
    private static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, ConsoleNotificationService>();

        return services;
    }

    private static IServiceCollection AddSecurity(
        this IServiceCollection services, IConfiguration configuration)
    {
        
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<
            Microsoft.AspNetCore.Identity.IPasswordHasher<User>,
            Microsoft.AspNetCore.Identity.PasswordHasher<User>>();

        services.AddAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration
                    .GetSection("Jwt")
                    .Get<JwtOptions>()!;
                 
                if (string.IsNullOrWhiteSpace(jwtOptions.Key))
                {
                    throw new InvalidOperationException(
                        "JWT key is not configured. Configure it using User Secrets or environment variables.");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

        return services;
    }
}
