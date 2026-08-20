using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Desktop.DTOs.Reminder;

public record UpdateReminderRequestDto
(
    [property: Required, DataType(DataType.DateTime)]
    DateTime NotifyAt
);