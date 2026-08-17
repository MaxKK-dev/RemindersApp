using NotesReminders.Application.DTOs.Note;
using NotesReminders.Application.DTOs.Reminder;
using NotesReminders.Application.Exceptions;
using NotesReminders.Application.Interfaces;
using NotesReminders.Application.Mappings;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepo;
    private readonly IReminderRepository _reminderRepo;
    public NoteService(INoteRepository noteRepo, IReminderRepository reminderRepository)
    {
        _noteRepo = noteRepo;
        _reminderRepo = reminderRepository;
    }
    public async Task<IReadOnlyList<NoteResponseDto>> GetAllAsync(int userId)
    {
        var notes = await _noteRepo.GetAllAsync(userId);
        return notes.Select(n => n.ToResponseDto()).ToList();
    }
    public async Task<IReadOnlyList<DeletedNoteResponseDto>> GetAllDeletedAsync(int userId)
    {
        var notes = await _noteRepo.GetAllDeletedAsync(userId);
        return notes.Select(n => n.ToDeletedResponseDto()).ToList();
    }

    public async Task<NoteResponseDto?> GetByIdAsync(int id, int userId)
    {
        var note = await GetExistingNoteAsync(id, userId);

        return note.ToResponseDto();   
    }

    public async Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto request, int userId)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId
        };

        await _noteRepo.AddAsync(note);
        await _noteRepo.SaveChangesAsync();

        return note.ToResponseDto();
    }

    public async Task<NoteResponseDto?> UpdateAsync(int id, UpdateNoteRequestDto request, int userId)
    {
        var note = await GetExistingNoteAsync(id, userId);

        note.Title = request.Title;
        note.Content = request.Content;
        note.UpdatedAt = DateTime.UtcNow;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();
    }
    public async Task<NoteResponseDto> CompleteAsync(int id, int userId)
    {
        var note = await GetExistingNoteAsync(id, userId);
    
        note.IsCompleted = true;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();
    }
    public async Task<NoteResponseDto> UnCompleteAsync(int id, int userId)
    {
        var note = await GetExistingNoteAsync(id, userId);

        note.IsCompleted = false;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();
    }

    public async Task<NoteResponseDto> DeleteAsync(int id, int userId)
    {
        var note = await GetExistingNoteAsync(id, userId);

        note.DeletedAt = DateTime.UtcNow;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();
    }
    public async Task<NoteResponseDto> RestoreAsync(int id, int userId)
    {
        var note = await _noteRepo.GetDeletedNoteByIdAsync(id, userId);
        if(note == null)
        {
            throw new NoteNotFoundException(id);
        }
        note.DeletedAt = null;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();

    }
    public async Task HardDeleteAsync(int id, int userId)
    {
        var note = await _noteRepo.GetDeletedNoteByIdAsync(id, userId);
        if(note == null)
        {
            throw new NoteNotFoundException(id);
        }
        _noteRepo.RemoveNote(note);
        await _noteRepo.SaveChangesAsync();
    }
    // Reminder operation
    public async Task<ReminderResponseDto> AddReminderAsync(int noteId,
    CreateReminderRequestDto request, int userId)
    {
        var note = GetExistingNoteAsync(noteId, userId);

        var reminder = new Reminder
        {
            NotifyAt = request.NotifyAt,
            NoteId = noteId
        };

        await _reminderRepo.AddAsync(reminder);
        await _reminderRepo.SaveChangesAsync();

        return reminder.ToResponseDto();
    }
    public async Task<ReminderResponseDto> UpdateReminderAsync(
        int noteId,
        int reminderId,
        UpdateReminderRequestDto request,
        int userId)
    {
        var note = await GetExistingNoteAsync(noteId, userId);

        var reminder = note.Reminders
            .FirstOrDefault(r => r.Id == reminderId);

        if (reminder is null)
            throw new ReminderNotFoundException(reminderId);

        reminder.NotifyAt = request.NotifyAt;

        await _reminderRepo.SaveChangesAsync();

        return reminder.ToResponseDto();
    }
    public async Task DeleteReminderAsync(
        int noteId,
        int reminderId,
        int userId)
    {
        var note = await GetExistingNoteAsync(noteId, userId);

        var reminder = note.Reminders
            .FirstOrDefault(r => r.Id == reminderId);

        if (reminder is null)
            throw new ReminderNotFoundException(reminderId);

        _reminderRepo.Remove(reminder);

        await _reminderRepo.SaveChangesAsync();
    }
// Helper methods
    private async Task<Note> GetExistingNoteAsync(int noteId, int userId)
    {
        var note = await _noteRepo.GetNoteByIdAsync(noteId, userId);

        if (note is null)
        {   
            throw new NoteNotFoundException(noteId);
        }

        return note;
}
}