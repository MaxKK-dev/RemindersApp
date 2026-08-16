using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NotesReminders.Application.Exceptions;

namespace NotesReminders.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unprocessed exception occurred: {message}", exception.Message);

        var isDevelopment = httpContext.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment();

        var problemDetails = exception switch
        {
            UserAlreadyExistsException => CreateProblemDetails(
                httpContext,
                StatusCodes.Status409Conflict,
                "User already exists",
                exception.Message),

            InvalidCredentialsException => CreateProblemDetails(
                httpContext,
                StatusCodes.Status401Unauthorized,
                "Invalid credentials",
                exception.Message),

            NoteNotFoundException => CreateProblemDetails(
                httpContext,
                StatusCodes.Status404NotFound,
                "Note not found",
                exception.Message),

            _ => CreateProblemDetails(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                isDevelopment ? exception.Message : "An unexpected error occurred")
        };

        httpContext.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = detail,
            Instance = httpContext.Request.Path
        };
    }
}
