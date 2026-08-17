using System.ComponentModel.DataAnnotations;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.DTOs.Reminder;

public record ReminderResponseDto
(
    int Id,
    int NoteId,
    DateTime NotifyAt
);