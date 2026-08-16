using NotesReminders.Application.DTOs.Note;

namespace NotesReminders.Application.Interfaces;

public interface INoteService
{
    Task<IReadOnlyList<NoteResponseDto>> GetAllAsync(int userId);

    Task<NoteResponseDto?> GetByIdAsync(int id, int userId);

    Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto request, int userId);

    Task<NoteResponseDto?> UpdateAsync(int id, UpdateNoteRequestDto request, int userId);

    Task<bool> DeleteAsync(int id, int userId);
}
