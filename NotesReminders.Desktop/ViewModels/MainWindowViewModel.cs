using CommunityToolkit.Mvvm.ComponentModel;

namespace NotesReminders.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? currentViewModel;
}