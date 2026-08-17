namespace NotesReminders.Domain.Entities;
public class Note
{
    public int Id {get; set; }
    public string Title {get; set; } = string.Empty;
    public string Content {get; set; } = string.Empty;
    public DateTime CreatedAt {get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt {get; set; }
    public DateTime? DeletedAt {get; set; }
    public bool IsCompleted {get; set; }

    public int UserId {get; set; }
    public User User {get; set; } = null!; 

    public List<Reminder> Reminders {get; set; } = [];
}