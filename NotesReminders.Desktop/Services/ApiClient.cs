using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using NotesReminders.Desktop.Common;

namespace NotesReminders.Desktop.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenStorage _tokenStorage;

    public ApiClient(
        HttpClient httpClient,
        TokenStorage tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    private void AddAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var token = _tokenStorage.GetToken();

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static ApiResult<T> CreateFailureResult<T>(HttpResponseMessage response)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest =>
                ApiResult<T>.Failure(
                    ApiError.Validation,
                    "The request is invalid."),

            HttpStatusCode.Unauthorized =>
                ApiResult<T>.Failure(
                    ApiError.Unauthorized,
                    "Unauthorized."),

            HttpStatusCode.NotFound =>
                ApiResult<T>.Failure(
                    ApiError.NotFound,
                    "Resource not found."),

            HttpStatusCode.InternalServerError =>
                ApiResult<T>.Failure(
                    ApiError.Server,
                    "Internal server error."),

            _ =>
                ApiResult<T>.Failure(
                    ApiError.Unknown,
                    response.ReasonPhrase)
        };
    }

    public async Task<ApiResult<TResponse>> GetAsync<TResponse>(string url)
    {
        AddAuthorizationHeader();

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return CreateFailureResult<TResponse>(response);

            var data = await response.Content.ReadFromJsonAsync<TResponse>();

            if (data is null)
            {
                return ApiResult<TResponse>.Failure(
                    ApiError.Unknown,
                    "Empty response.");
            }

            return ApiResult<TResponse>.Success(data);
        }
        catch (HttpRequestException)
        {
            return ApiResult<TResponse>.ConnectionFailure();
        }
    }

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request)
    {
        AddAuthorizationHeader();

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
                return CreateFailureResult<TResponse>(response);

            var data = await response.Content.ReadFromJsonAsync<TResponse>();

            if (data is null)
            {
                return ApiResult<TResponse>.Failure(
                    ApiError.Unknown,
                    "Empty response.");
            }

            return ApiResult<TResponse>.Success(data);
        }
        catch (HttpRequestException)
        {
            return ApiResult<TResponse>.ConnectionFailure();
        }
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
        string url,
        TRequest request)
    {
        AddAuthorizationHeader();

        try
        {
            var response = await _httpClient.PutAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
                return CreateFailureResult<TResponse>(response);

            var data = await response.Content.ReadFromJsonAsync<TResponse>();

            if (data is null)
            {
                return ApiResult<TResponse>.Failure(
                    ApiError.Unknown,
                    "Empty response.");
            }

            return ApiResult<TResponse>.Success(data);
        }
        catch (HttpRequestException)
        {
            return ApiResult<TResponse>.ConnectionFailure();
        }
    }

    public async Task<ApiResult<bool>> DeleteAsync(string url)
    {
        AddAuthorizationHeader();

        try
        {
            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
                return CreateFailureResult<bool>(response);

            return ApiResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.ConnectionFailure();
        }
    }
}