using NotesReminders.Desktop.Common;
using NotesReminders.Desktop.DTOs.Auth;

namespace NotesReminders.Desktop.Services;

public class AuthService
{
    private readonly ApiClient _apiClient;
    private readonly TokenStorage _tokenStorage;

    public AuthService(
        ApiClient apiClient,
        TokenStorage tokenStorage)
    {
        _apiClient = apiClient;
        _tokenStorage = tokenStorage;
    }

    public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var result =
            await _apiClient.PostAsync<LoginRequestDto, LoginResponseDto>(
                "api/auth/login",
                request);

        if (!result.IsSuccess)
            return result;

        if (result.Data is null)
        {
            return ApiResult<LoginResponseDto>.Failure(
                ApiError.Unknown,
                "Server returned no data.");
        }

        _tokenStorage.SaveToken(result.Data.Token);

        return result;
    }

    public async Task<ApiResult<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var result =
            await _apiClient.PostAsync<RegisterRequestDto, RegisterResponseDto>(
                "api/auth/register",
                request);

        if (!result.IsSuccess)
            return result;

        return result;
    }

    public void Logout()
    {
        _tokenStorage.Clear();
    }
}