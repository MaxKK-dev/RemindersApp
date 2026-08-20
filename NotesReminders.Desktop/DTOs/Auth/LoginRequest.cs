namespace NotesReminders.Desktop.DTOs.Auth;

public record LoginRequestDto
(
    string Username,
    string Password
);