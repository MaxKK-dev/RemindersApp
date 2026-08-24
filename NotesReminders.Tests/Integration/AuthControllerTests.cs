using System.Net;
using System.Net.Http.Json;

using NotesReminders.Application.DTOs.Auth;


namespace NotesReminders.Tests.Integration;

public class AuthControllerTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(ApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ReturnsToken()
    {
        var request = new RegisterRequestDto(
            "testuser",
            "Password123!"
        );

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal(request.Username, result.UserName);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ReturnsError()
    {
        var request = new RegisterRequestDto(
            "duplicateuser",
            "Password123!"
        );

        await _client.PostAsJsonAsync("/api/Auth/register", request);

        var response = await _client.PostAsJsonAsync("/api/Auth/register", request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsToken()
    {
        var register = new RegisterRequestDto(
            "loginuser",
            "Password123!"
        );

        await _client.PostAsJsonAsync("/api/Auth/register", register);

        var login = new LoginRequestDto(
            register.Username,
            register.Password);

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            login);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal(login.Username, result.UserName);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsError()
    {
        var register = new RegisterRequestDto(
            "wrongpassworduser",
            "Password123!");

        await _client.PostAsJsonAsync("/api/Auth/register", register);

        var login = new LoginRequestDto(
            register.Username,
            "WrongPassword123!");

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            login);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidUsername_ReturnsBadRequest()
    {
        var request = new RegisterRequestDto(
            "ab",
            "Password123!");

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidPassword_ReturnsBadRequest()
    {
        var request = new RegisterRequestDto(
            "validuser",
            "123");

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
