using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NotesReminders.Application.DTOs.Reminder;
using NotesReminders.Domain.Entities;
using NotesReminders.Infrastructure.Data;

namespace NotesReminders.Tests.Integration;

public class ReminderControllerTests :
    IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public ReminderControllerTests(ApiTestFactory factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task CreateReminder_WhenAuthenticated_CreatesReminder()
    {
        await _factory.ClearDatabaseAsync();

        await _factory.SeedAsync(
            new Note
            {
                Id = 1,
                UserId = 1,
                Title = "Reminder note",
                Content = "Test"
            });

        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User-Id", "1");

        var request = new CreateReminderRequestDto
        {
            NoteId = 1,
            NotifyAt = DateTime.UtcNow.AddHours(1)
        };

        var response = await client.PostAsJsonAsync("/api/notes/1/reminders", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var reminder = await response.Content.ReadFromJsonAsync<ReminderResponseDto>();

        Assert.NotNull(reminder);
        Assert.Equal(1, reminder.NoteId);
        Assert.Equal(request.NotifyAt,
            DateTime.SpecifyKind(reminder.NotifyAt, DateTimeKind.Utc));

    }


    [Fact]
    public async Task UpdateReminder_WhenAuthenticated_UpdatesReminder()
    {
        await _factory.ClearDatabaseAsync();

        await _factory.SeedAsync(
            new Note
            {
                Id = 1,
                UserId = 1,
                Title = "Reminder note",
                Content = "Test"
            });


        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();


        db.Reminders.Add(
            new Reminder
            {
                Id = 1,
                NoteId = 1,
                NotifyAt = DateTime.UtcNow.AddHours(1)
            });

        await db.SaveChangesAsync();


        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User-Id", "1");


        var newDate =
            DateTime.UtcNow.AddHours(5);


        var request = new UpdateReminderRequestDto
        {
            NotifyAt = newDate
        };


        var response = await client.PutAsJsonAsync(
            "/api/notes/1/reminders/1", request);


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var reminder = await db.Reminders.AsNoTracking().FirstAsync(x => x.Id == 1);

        Assert.True(Math.Abs((newDate - reminder.NotifyAt).TotalSeconds) < 1);    
    }


    [Fact]
    public async Task DeleteReminder_WhenAuthenticated_DeletesReminder()
    {
        await _factory.ClearDatabaseAsync();

        await _factory.SeedAsync(
            new Note
            {
                Id = 1,
                UserId = 1,
                Title = "Reminder note",
                Content = "Test"
            });


        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();


        db.Reminders.Add(
            new Reminder
            {
                Id = 1,
                NoteId = 1,
                NotifyAt = DateTime.UtcNow.AddHours(1)
            });

        await db.SaveChangesAsync();


        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User-Id", "1");

        var response = await client.DeleteAsync("/api/notes/1/reminders/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var reminderExists = await db.Reminders.AnyAsync(x => x.Id == 1);

        Assert.False(reminderExists);
    }


    [Fact]
    public async Task CreateReminder_WhenNoteBelongsToAnotherUser_ReturnsNotFound()
    {
        await _factory.ClearDatabaseAsync();


        await _factory.SeedAsync(
            new Note
            {
                Id = 1,
                UserId = 2,
                Title = "Private note",
                Content = "Secret"
            });


        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User-Id", "1");

        var request = new CreateReminderRequestDto
        {
            NoteId = 1,
            NotifyAt = DateTime.UtcNow.AddHours(1)
        };


        var response = await client.PostAsJsonAsync("/api/notes/1/reminders", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}