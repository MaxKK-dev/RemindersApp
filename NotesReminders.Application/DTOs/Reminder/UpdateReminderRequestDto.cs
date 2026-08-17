using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Application.DTOs.Reminder;

public record UpdateReminderRequestDto
(
    [property: Required, DataType(DataType.DateTime)]
    DateTime NotifyAt
);