using NotesReminders.Domain.Entities;
using NotesReminders.Application.DTOs.Reminder;

namespace NotesReminders.Application.DTOs.Note;

public record NoteResponseDto(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsComlete,
    ICollection<ReminderResponseDto> Reminders

);