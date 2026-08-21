using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NotesReminders.Domain.Entities;

using NotesReminders.Infrastructure.Data;

namespace NotesReminders.Tests.Integration;

public class ApiTestFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;

    public ApiTestFactory()
    {
        var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        db.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove background services
            services.RemoveAll<Microsoft.Extensions.Hosting.IHostedService>();

            // Remove production database
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            // Create SQLite in-memory database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Replace JWT authentication with test authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    TestAuthenticationHandler.SchemeName;

                options.DefaultChallengeScheme =
                    TestAuthenticationHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.SchemeName,
                _ => { });
        });
    }

    public async Task SeedAsync(params Note[] notes)
    {
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var users = notes
            .Select(x => x.UserId)
            .Distinct()
            .Select(id => new User
            {
                Id = id,
                Username = $"user{id}",
                PasswordHash = "test"
            });

        await db.Users.AddRangeAsync(users);

        await db.Notes.AddRangeAsync(notes);

        await db.SaveChangesAsync();
    }

    public async Task ClearDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        db.Reminders.RemoveRange(db.Reminders);
        db.Notes.RemoveRange(db.Notes);
        db.Users.RemoveRange(db.Users);

        await db.SaveChangesAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}


public sealed class TestAuthenticationHandler 
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "TestAuthentication";

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userId = Request.Headers["X-Test-User-Id"]
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        if (!int.TryParse(userId, out var parsedUserId))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Invalid test user id."));
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                parsedUserId.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            SchemeName);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            SchemeName);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}