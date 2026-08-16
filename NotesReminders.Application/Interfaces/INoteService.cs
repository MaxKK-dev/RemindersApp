using NotesReminders.Application.DTOs.Note;

namespace NotesReminders.Application.Interfaces;

public interface INoteService
{
    Task<IReadOnlyList<NoteResponseDto>> GetAllAsync(int userId);
    Task<IReadOnlyList<DeletedNoteResponseDto>> GetAllDeletedAsync(int userId);

    Task<NoteResponseDto?> GetByIdAsync(int id, int userId);

    Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto request, int userId);

    Task<NoteResponseDto?> UpdateAsync(int id, UpdateNoteRequestDto request, int userId);

    Task<NoteResponseDto> DeleteAsync(int id, int userId);
    Task<NoteResponseDto> RestoreAsync(int id, int userId);
    Task<NoteResponseDto> CompleteAsync(int id, int userId);
    Task<NoteResponseDto> UnCompleteAsync(int id, int userId);
    Task HardDeleteAsync (int id, int userId);
}
