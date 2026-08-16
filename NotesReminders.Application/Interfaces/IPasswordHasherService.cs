using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Interfaces;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password);
}