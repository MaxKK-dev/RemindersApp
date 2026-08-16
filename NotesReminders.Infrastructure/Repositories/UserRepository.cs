using NotesReminders.Domain.Entities;
using NotesReminders.Application.Interfaces;
using NotesReminders.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace NotesReminders.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public User Add(User user)
    {
        _dbContext.Users.Add(user);
        return user;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}