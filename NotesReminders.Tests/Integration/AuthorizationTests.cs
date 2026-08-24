using System.Net;
using System.Net.Http.Json;

using NotesReminders.Application.DTOs.Note;
using NotesReminders.Application.DTOs.Reminder;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Tests.Integration;

public class AuthorizationTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public AuthorizationTests(ApiTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task User_Cannot_Get_AnotherUsersNote()
    {
        await _factory.ClearDatabaseAsync();

        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "User 1 note",
            Content = "Private content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(2);

        var response = await client.GetAsync("/api/Notes/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_Cannot_Update_AnotherUsersNote()
    {
        await _factory.ClearDatabaseAsync();

        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "User 1 note",
            Content = "Original content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(2);

        var request = new UpdateNoteRequestDto{
            Title = "Changed title",
            Content = "Changed content"
        };

        var response = await client.PutAsJsonAsync(
            "/api/Notes/1",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_Cannot_Delete_AnotherUsersNote()
    {
        await _factory.ClearDatabaseAsync();

        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "User 1 note",
            Content = "Private content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(2);

        var response = await client.DeleteAsync("/api/Notes/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_Cannot_AddReminder_To_AnotherUsersNote()
    {
        await _factory.ClearDatabaseAsync();

        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "User 1 note",
            Content = "Private content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(2);

        var request = new CreateReminderRequestDto
        {
            NoteId = 1,
            NotifyAt = DateTime.UtcNow.AddHours(1)
        };

        var response = await client.PostAsJsonAsync(
            "/api/Notes/1/reminder",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_Cannot_Update_AnotherUsersReminder()
    {
        await _factory.ClearDatabaseAsync();

        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "User 1 note",
            Content = "Private content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Reminders =
            [
                new Reminder
                {
                    Id = 1,
                    NoteId = 1,
                    NotifyAt = DateTime.UtcNow.AddHours(1)
                }
            ]
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(2);

        var request = new UpdateReminderRequestDto
        {
            NotifyAt = DateTime.UtcNow.AddHours(2)
        };                          

        var response = await client.PutAsJsonAsync(
            "/api/Reminders/1",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_Cannot_Delete_AnotherUsersReminder()
    {
        await _factory.ClearDatabaseAsync();

       var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "User 1 note",
            Content = "Private content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Reminders =
            [
                new Reminder
                {
                    Id = 1,
                    NoteId = 1,
                    NotifyAt = DateTime.UtcNow.AddHours(1)
                }
            ]
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(2);

        var response = await client.DeleteAsync(
            "/api/Reminders/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task User_Can_Get_OwnNote()
    {
        await _factory.ClearDatabaseAsync();

        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "My note",
            Content = "My content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(1);

        var response = await client.GetAsync("/api/Notes/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task User_Can_Update_OwnNote()
    {
        await _factory.ClearDatabaseAsync();

        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "Original title",
            Content = "Original content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _factory.SeedAsync(note);

        using var client = CreateClient(1);

        var request = new UpdateNoteRequestDto{
            Title = "Updated title",
            Content = "Updated content"
        };

        var response = await client.PutAsJsonAsync(
            "/api/Notes/1",
            request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private HttpClient CreateClient(int userId)
    {
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            userId.ToString());

        return client;
    }
}