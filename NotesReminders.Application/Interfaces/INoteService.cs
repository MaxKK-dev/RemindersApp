using NotesReminders.Application.DTOs.Note;
using NotesReminders.Application.DTOs.Reminder;

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
    Task<ReminderResponseDto> AddReminderAsync(int noteId, CreateReminderRequestDto request, int userId);

    Task<ReminderResponseDto> UpdateReminderAsync(int noteId, int reminderId, UpdateReminderRequestDto request, int userId);

    Task DeleteReminderAsync(int noteId, int reminderId, int userId);
}
