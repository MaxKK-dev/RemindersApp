using System.Security.Claims;

namespace NotesReminders.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "User identifier claim is missing or invalid.");
        }

        return userId;
    }
}