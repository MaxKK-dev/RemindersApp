using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
