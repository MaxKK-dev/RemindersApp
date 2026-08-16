using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Interfaces;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync(int userId);
    Task<Note?> GetNoteByIdAsync(int id, int userId);
    Task<Note> CreateAsync(Note note);
    Task<Note?> UpdateAsync(Note note);
    Task SaveChangesAsync();
    Task<bool> DeleteAsync(int id, int userId);
}
