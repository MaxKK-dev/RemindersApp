using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Interfaces;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync(int userId);
    Task<IEnumerable<Note>> GetAllDeletedAsync(int userId);
    Task<Note?> GetNoteByIdAsync(int id, int userId);
    Task<Note?> GetDeletedNoteByIdAsync(int id, int userId);
    Task AddAsync(Note note);
    void RemoveNote(Note note);
    Task SaveChangesAsync();
}
