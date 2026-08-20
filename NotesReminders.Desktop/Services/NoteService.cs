using NotesReminders.Desktop.Common;
using NotesReminders.Desktop.DTOs.Note;
using NotesReminders.Desktop.DTOs.Reminder;

namespace NotesReminders.Desktop.Services;

public class NoteService
{
    private readonly ApiClient _apiClient;

    public NoteService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResult<IReadOnlyCollection<NoteResponseDto>>> GetNotesAsync()
    {
        return await _apiClient.GetAsync<IReadOnlyCollection<NoteResponseDto>>(
            "api/notes");
    }

    public async Task<ApiResult<NoteResponseDto>> CreateNoteAsync(
        CreateNoteRequestDto request)
    {
        return await _apiClient.PostAsync<CreateNoteRequestDto, NoteResponseDto>(
            "api/notes",
            request);
    }

    public async Task<ApiResult<NoteResponseDto>> UpdateNoteAsync(
        int id,
        UpdateNoteRequestDto request)
    {
        return await _apiClient.PutAsync<UpdateNoteRequestDto, NoteResponseDto>(
            $"api/notes/{id}",
            request);
    }

    public async Task<ApiResult<bool>> DeleteNoteAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/notes/{id}");
    }
    public async Task<ApiResult<ReminderResponseDto>> AddReminderAsync(
        int noteId,
        DateTime notifyAt)
    {
        var request = new CreateReminderRequestDto(
            noteId,
            notifyAt);

        return await _apiClient.PostAsync<CreateReminderRequestDto, ReminderResponseDto>(
            $"api/notes/{noteId}/reminders",
            request);
    }

    public async Task<ApiResult<ReminderResponseDto>> UpdateReminderAsync(
        int noteId,
        int reminderId,
        DateTime notifyAt)
    {
        var request = new UpdateReminderRequestDto(
            notifyAt);

        return await _apiClient.PutAsync<UpdateReminderRequestDto, ReminderResponseDto>(
            $"api/notes/{noteId}/reminders/{reminderId}",
            request);
    }

    public async Task<ApiResult<bool>> DeleteReminderAsync(
        int noteId,
        int reminderId)
    {
        return await _apiClient.DeleteAsync(
            $"api/notes/{noteId}/reminders/{reminderId}");
}
}