using NotesReminders.Application.DTOs.Note;
using NotesReminders.Application.Exceptions;
using NotesReminders.Application.Interfaces;
using NotesReminders.Application.Mappings;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepo;
    public NoteService(INoteRepository noteRepo)
    {
        _noteRepo = noteRepo;
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
        var note = await _noteRepo.GetNoteByIdAsync(id, userId);
        if(note == null)
        {
            throw new NoteNotFoundException(id);
        }
        return note.ToResponseDto();   
    }

    public async Task<NoteResponseDto> CreateAsync(CreateNoteRequestDto request, int userId)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            ReminderTime = request.ReminderTime,
            UserId = userId
        };

        var createdNote = await _noteRepo.AddAsync(note);
        await _noteRepo.SaveChangesAsync();

        return createdNote.ToResponseDto();
    }

    public async Task<NoteResponseDto?> UpdateAsync(int id, UpdateNoteRequestDto request, int userId)
    {
        var note = await _noteRepo.GetNoteByIdAsync(id, userId);
        if(note == null)
        {
            throw new NoteNotFoundException(id);
        }
        note.Title = request.Title;
        note.Content = request.Content;
        note.ReminderTime = request.ReminderTime;
        note.UpdatedAt = DateTime.UtcNow;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();
    }
    public async Task<NoteResponseDto> CompleteAsync(int id, int userId)
    {
        var note = await _noteRepo.GetNoteByIdAsync(id, userId);
        if(note == null)
        {
            throw new NoteNotFoundException(id);
        }
        note.IsCompleted = true;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();
    }
    public async Task<NoteResponseDto> UnCompleteAsync(int id, int userId)
    {
        var note = await _noteRepo.GetNoteByIdAsync(id, userId);
        if(note == null)
        {
            throw new NoteNotFoundException(id);
        }
        note.IsCompleted = false;
        await _noteRepo.SaveChangesAsync();
        return note.ToResponseDto();
    }

    public async Task<NoteResponseDto> DeleteAsync(int id, int userId)
    {
        var note = await _noteRepo.GetNoteByIdAsync(id, userId);
        if(note == null)
        {
            throw new NoteNotFoundException(id);
        }
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


}
