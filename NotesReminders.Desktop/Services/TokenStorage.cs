namespace NotesReminders.Desktop.Services;

public class TokenStorage
{
    private string? _token;

    public string? GetToken()
    {
        return _token;
    }

    public void SaveToken(string token)
    {
        _token = token;
    }

    public void Clear()
    {
        _token = null;
    }
}