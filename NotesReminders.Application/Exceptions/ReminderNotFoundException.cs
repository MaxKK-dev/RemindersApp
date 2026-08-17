public sealed class ReminderNotFoundException : Exception
{
    public int ReminderId { get; }

    public ReminderNotFoundException(int reminderId)
        : base($"Reminder with id '{reminderId}' was not found.")
    {
        ReminderId = reminderId;
    }
}