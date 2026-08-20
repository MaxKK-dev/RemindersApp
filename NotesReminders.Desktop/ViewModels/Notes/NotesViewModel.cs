using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotesReminders.Desktop.DTOs.Note;
using NotesReminders.Desktop.Navigation;
using NotesReminders.Desktop.Services;

namespace NotesReminders.Desktop.ViewModels.Notes;

public partial class NotesViewModel : ViewModelBase, INavigationAware
{
    private readonly NoteService _noteService;

    public ObservableCollection<NoteResponseDto> Notes { get; } = [];

    public ObservableCollection<ReminderItemViewModel> ReminderItems { get; } = [];

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isCreatingNote;

    [ObservableProperty]
    private bool isEditingNote;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string content = string.Empty;

    [ObservableProperty]
    private int selectedNoteId;

    [ObservableProperty]
    private NoteResponseDto? selectedNote;

    [ObservableProperty]
    private DateTimeOffset? newReminderDate;

    [ObservableProperty]
    private TimeSpan? newReminderTime;

    public NotesViewModel(NoteService noteService)
    {
        _noteService = noteService;
    }

    public async Task OnNavigatedToAsync()
    {
        await LoadNotesAsync();
    }

    partial void OnSelectedNoteChanged(NoteResponseDto? value)
    {
        if (value == null)
            return;

        Title = value.Title;
        Content = value.Content;
        SelectedNoteId = value.Id;

        LoadReminderItems(value);

        IsEditingNote = true;
        IsCreatingNote = false;
    }

    private void LoadReminderItems(NoteResponseDto note)
    {
        ReminderItems.Clear();

        foreach (var reminder in note.Reminders.OrderBy(r => r.NotifyAt))
        {
            ReminderItems.Add(new ReminderItemViewModel(reminder));
        }

        ClearNewReminder();
    }

    public async Task LoadNotesAsync()
    {
        if (IsLoading)
            return;

        IsLoading = true;
        ErrorMessage = string.Empty;

        var result = await _noteService.GetNotesAsync();

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to load notes.";
            IsLoading = false;
            return;
        }

        Notes.Clear();

        if (result.Data != null)
        {
            foreach (var note in result.Data)
            {
                Notes.Add(note);
            }
        }

        IsLoading = false;
    }

    [RelayCommand]
    private void NewNote()
    {
        Title = string.Empty;
        Content = string.Empty;
        ErrorMessage = string.Empty;

        SelectedNote = null;
        SelectedNoteId = 0;

        ReminderItems.Clear();
        ClearNewReminder();

        IsCreatingNote = true;
        IsEditingNote = false;
    }

    [RelayCommand]
    private void CancelCreate()
    {
        IsCreatingNote = false;
        IsEditingNote = false;

        SelectedNote = null;
        SelectedNoteId = 0;

        ReminderItems.Clear();
        ClearNewReminder();
    }

    [RelayCommand]
    private async Task CreateNoteAsync()
    {
        ErrorMessage = string.Empty;

        var request = new CreateNoteRequestDto
        (
            Title = Title,
            Content = Content
        );

        var result = await _noteService.CreateNoteAsync(request);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to create note.";
            return;
        }

        IsCreatingNote = false;

        Title = string.Empty;
        Content = string.Empty;

        ReminderItems.Clear();
        ClearNewReminder();

        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        ErrorMessage = string.Empty;

        var request = new UpdateNoteRequestDto
        (
            Title = Title,
            Content = Content
        );

        var result = await _noteService.UpdateNoteAsync(
            SelectedNoteId,
            request);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to update note.";
            return;
        }

        IsEditingNote = false;

        Title = string.Empty;
        Content = string.Empty;

        SelectedNote = null;
        SelectedNoteId = 0;

        ReminderItems.Clear();
        ClearNewReminder();

        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task DeleteNoteAsync()
    {
        ErrorMessage = string.Empty;

        var result = await _noteService.DeleteNoteAsync(SelectedNoteId);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to delete note.";
            return;
        }

        IsEditingNote = false;

        Title = string.Empty;
        Content = string.Empty;

        SelectedNote = null;
        SelectedNoteId = 0;

        ReminderItems.Clear();
        ClearNewReminder();

        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task AddReminderAsync()
    {
        ErrorMessage = string.Empty;

        if (SelectedNoteId == 0)
        {
            ErrorMessage = "Save the note before adding a reminder.";
            return;
        }

        if (!NewReminderDate.HasValue || !NewReminderTime.HasValue)
        {
            ErrorMessage = "Select both a date and time for the reminder.";
            return;
        }

        var localDateTime = NewReminderDate.Value.Date
            .Add(NewReminderTime.Value);

        var notifyAt = DateTime.SpecifyKind(
            localDateTime,
            DateTimeKind.Local)
            .ToUniversalTime();

        var result = await _noteService.AddReminderAsync(
            SelectedNoteId,
            notifyAt);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to add reminder.";
            return;
        }

        if (result.Data != null)
        {
            ReminderItems.Add(
                new ReminderItemViewModel(result.Data));

            var ordered = ReminderItems
                .OrderBy(r => r.Date)
                .ThenBy(r => r.Time)
                .ToList();

            ReminderItems.Clear();

            foreach (var reminder in ordered)
            {
                ReminderItems.Add(reminder);
            }
        }

        ClearNewReminder();
    }

    [RelayCommand]
    private async Task UpdateReminderAsync(ReminderItemViewModel? reminder)
    {
        if (reminder == null)
            return;

        ErrorMessage = string.Empty;

        if (!reminder.Date.HasValue)
        {
            ErrorMessage = "Select a date for the reminder.";
            return;
        }

        DateTime localDateTime = reminder.Date.Value.DateTime.Add(reminder.Time);

        var notifyAt = DateTime.SpecifyKind(
            localDateTime,
            DateTimeKind.Local)
            .ToUniversalTime();

        var result = await _noteService.UpdateReminderAsync(
            reminder.NoteId,
            reminder.Id,
            notifyAt);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to update reminder.";
            return;
        }

        if (result.Data != null)
        {
            var local = result.Data.NotifyAt.ToLocalTime();

            reminder.Date = new DateTimeOffset(local.Date);
            reminder.Time = local.TimeOfDay;
        }
    }

    [RelayCommand]
    private async Task DeleteReminderAsync(ReminderItemViewModel? reminder)
    {
        if (reminder == null)
            return;

        ErrorMessage = string.Empty;

        var result = await _noteService.DeleteReminderAsync(
            reminder.NoteId,
            reminder.Id);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to delete reminder.";
            return;
        }

        ReminderItems.Remove(reminder);
    }

    private void ClearNewReminder()
    {
        NewReminderDate = null;
        NewReminderTime = null;
    }
}