
namespace NotesReminders.Application.DTOs.Note;

public record NoteResponseDto(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? ReminderTime
);