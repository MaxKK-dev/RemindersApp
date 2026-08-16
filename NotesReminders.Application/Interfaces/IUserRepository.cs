using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    User Add(User user);
    Task SaveChangesAsync();

}