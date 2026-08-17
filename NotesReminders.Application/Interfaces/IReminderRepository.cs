using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Interfaces;

public interface IReminderRepository
{   
    Task AddAsync(Reminder reminder);
    Task AddRangeAsync(IEnumerable<Reminder> reminders);
    Task<List<Reminder>> GetDueRemindersAsync(DateTime utcNow);
    void Remove(Reminder reminder);
    void RemoveRange(IEnumerable<Reminder> reminders);
    Task RemoveByNoteIdAsync(int noteId);
    Task SaveChangesAsync();
}
