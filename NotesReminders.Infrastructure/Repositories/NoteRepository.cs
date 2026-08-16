
using NotesReminders.Application.Interfaces;
using NotesReminders.Domain.Entities;
using NotesReminders.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace NotesReminders.Infrastructure.Repositories;
public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Note> _notes;

    public NoteRepository(AppDbContext context)
    {
        _context = context;
        _notes = context.Set<Note>();
    }

    public async Task<IEnumerable<Note>> GetAllAsync(int userId)
    {
        return await _notes
            .Where(n => n.UserId == userId && n.DeletedAt == null)
            .ToListAsync();
    }
    public async Task<IEnumerable<Note>> GetAllDeletedAsync(int userId)
    {
        return await _notes.Where(n => n.UserId == userId && n.DeletedAt != null).ToListAsync();
    }

    public async Task<Note?> GetNoteByIdAsync(int id, int userId)
    {
        return await _notes
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id && n.DeletedAt == null);
    }
    public async Task<Note?> GetDeletedNoteByIdAsync(int id, int userId)
    {
        return await _notes
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == id && n.DeletedAt != null);
    }
    public async Task<Note> AddAsync(Note note)
    {
        await _notes.AddAsync(note);
        return note;
    }
    public void RemoveNote(Note note)
    {
        _notes.Remove(note);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

}
