using NotesReminders.Application.DTOs.Reminder;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Mappings;

public static class ReminderMapper
{
    public static ReminderResponseDto ToResponseDto(this Reminder reminder)
    {
        return new ReminderResponseDto
        (
            reminder.Id,
            reminder.NoteId,
            reminder.NotifyAt
        );
    }
}