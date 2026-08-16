using NotesReminders.Domain.Entities;
using NotesReminders.Application.Interfaces;

using Microsoft.AspNetCore.Identity;

namespace NotesReminders.Infrastructure.Security;

public class PasswordHasherService : IPasswordHasherService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    public PasswordHasherService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }
    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }
    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}