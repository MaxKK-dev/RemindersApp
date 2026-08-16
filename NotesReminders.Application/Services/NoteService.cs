using NotesReminders.Application.DTOs.Note;
using NotesReminders.Application.Interfaces;

namespace NotesReminders.Application.Services;

public class NoteService : INoteService
{
    public Task<IReadOnlyList<NoteResponseDto>> GetAllAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<NoteResponseDto?> GetByIdAsync(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto request, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<NoteResponseDto?> UpdateAsync(int id, UpdateNoteRequestDto request, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id, int userId)
    {
        throw new NotImplementedException();
    }
}
