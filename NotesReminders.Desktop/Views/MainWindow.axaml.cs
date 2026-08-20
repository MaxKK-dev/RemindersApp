using Avalonia.Controls;

using NotesReminders.Desktop.ViewModels;

namespace NotesReminders.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();    
    }
}