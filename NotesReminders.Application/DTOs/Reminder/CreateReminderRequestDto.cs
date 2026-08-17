using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Application.DTOs.Reminder;

public record CreateReminderRequestDto
(
    [property: Required]
    int NoteId,
    [property: Required, DataType(DataType.DateTime)]
    DateTime NotifyAt
);