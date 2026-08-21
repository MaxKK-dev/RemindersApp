using System.Net;
using System.Net.Http.Json;

using NotesReminders.Application.DTOs.Note;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Tests.Integration;

public class NotesControllerTests :
    IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;

    public NotesControllerTests(ApiTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllNotes_WhenAuthenticated_ReturnsOnlyUsersNotes()
    {
        await _factory.ClearDatabaseAsync();

        await _factory.SeedAsync(
            new Note
            {
                Id = 1,
                UserId = 1,
                Title = "User 1 note",
                Content = "Visible"
            },
            new Note
            {
                Id = 2,
                UserId = 2,
                Title = "User 2 note",
                Content = "Should not be visible"
            });

        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            "1");

        var response = await client.GetAsync("/api/notes");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var notes =
            await response.Content.ReadFromJsonAsync<
                List<NoteResponseDto>>();

        Assert.NotNull(notes);
        Assert.Single(notes);

        Assert.Equal(
            "User 1 note",
            notes[0].Title);
    }

    [Fact]
    public async Task GetNoteById_WhenNoteBelongsToUser_ReturnsNote()
    {
        await _factory.ClearDatabaseAsync();

        await _factory.SeedAsync(
            new Note
            {
                Id = 1,
                UserId = 1,
                Title = "Test note",
                Content = "Test content"
            });

        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            "1");

        var response =
            await client.GetAsync("/api/notes/1");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var note =
            await response.Content.ReadFromJsonAsync<
                NoteResponseDto>();

        Assert.NotNull(note);
        Assert.Equal(1, note.Id);
        Assert.Equal("Test note", note.Title);
        Assert.Equal("Test content", note.Content);
    }

    [Fact]
    public async Task GetNoteById_WhenNoteBelongsToAnotherUser_ReturnsNotFound()
    {
        await _factory.ClearDatabaseAsync();

        await _factory.SeedAsync(
            new Note
            {
                Id = 1,
                UserId = 1,
                Title = "Private note",
                Content = "Private"
            });

        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            "2");

        var response =
            await client.GetAsync("/api/notes/1");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetAllNotes_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        await _factory.ClearDatabaseAsync();

        using var client = _factory.CreateClient();

        var response =
            await client.GetAsync("/api/notes");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateNote_WhenAuthenticated_CreatesNote()
    {
        await _factory.ClearDatabaseAsync();
        
        await _factory.SeedAsync(new Note
            {
                UserId = 1,
                Title = "Existing note",
                Content = "Creates user"
            });

        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            "1");

        var request = new CreateNoteRequestDto
        {
            Title = "Created through API",
            Content = "Created by integration test"
        };

        var response =
            await client.PostAsJsonAsync(
                "/api/notes",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var note =
            await response.Content.ReadFromJsonAsync<
                NoteResponseDto>();

        Assert.NotNull(note);
        Assert.Equal(
            "Created through API",
            note.Title);

        Assert.Equal(
            "Created by integration test",
            note.Content);
    }

    [Fact]
    public async Task CreateNote_WhenTitleIsTooShort_ReturnsBadRequest()
    {
        await _factory.ClearDatabaseAsync();

        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            "1");

        var request = new CreateNoteRequestDto
        {
            Title = "Hi",
            Content = "Invalid title"
        };

        var response =
            await client.PostAsJsonAsync(
                "/api/notes",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}