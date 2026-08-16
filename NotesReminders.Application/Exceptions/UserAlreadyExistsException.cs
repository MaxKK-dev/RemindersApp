namespace NotesReminders.Application.Exceptions;

public sealed class UserAlreadyExistsException : Exception
{
    public string Username { get; }

    public UserAlreadyExistsException(string username)
        : base($"User '{username}' already exists.")
    {
        Username = username;
    }
}