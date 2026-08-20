namespace NotesReminders.Desktop.Navigation;

public interface INavigationAware
{
    Task OnNavigatedToAsync();
}