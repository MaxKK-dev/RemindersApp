using NotesReminders.Application.Interfaces;
using NotesReminders.Infrastructure.Data;
using NotesReminders.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace NotesReminders.Infrastructure.Repositories;
public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    public ReminderRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Reminder reminder)
    {
        await _context.Reminders.AddAsync(reminder);
    }

    public async Task AddRangeAsync(IEnumerable<Reminder> reminders)
    {
        await _context.Reminders.AddRangeAsync(reminders);
    }

    public async Task<List<Reminder>> GetDueRemindersAsync(DateTime utcNow)
    {
        return await _context.Reminders
            .Include(r => r.Note)
            .Where(r => r.NotifyAt <= utcNow)
            .OrderBy(r => r.NotifyAt)
            .ToListAsync();
    }
    public void Remove(Reminder reminder)
    {
        _context.Reminders.Remove(reminder);
    }
    public void RemoveRange(IEnumerable<Reminder> reminders)
    {
        _context.Reminders.RemoveRange(reminders);
    }

    public async Task RemoveByNoteIdAsync(int noteId)
    {
        await _context.Reminders.Where(r => r.NoteId == noteId).ExecuteDeleteAsync();
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}