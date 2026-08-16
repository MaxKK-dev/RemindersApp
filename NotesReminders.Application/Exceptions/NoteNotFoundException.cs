namespace NotesReminders.Application.Exceptions;

public sealed class NoteNotFoundException : Exception
{
    public int NoteId { get; }

    public NoteNotFoundException(int noteId)
        : base($"Note with id '{noteId}' was not found.")
    {
        NoteId = noteId;
    }
}