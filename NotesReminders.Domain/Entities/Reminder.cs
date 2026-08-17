namespace NotesReminders.Domain.Entities;

public class Reminder
{
    public int Id{get; set; }
    public DateTime NotifyAt {get; set; }
    public int NoteId {get; set;}
    public Note Note {get; set; } = null!;

}