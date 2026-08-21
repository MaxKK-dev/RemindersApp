using Moq;
using NotesReminders.Application.DTOs.Note;
using NotesReminders.Application.DTOs.Reminder;
using NotesReminders.Application.Exceptions;
using NotesReminders.Application.Interfaces;
using NotesReminders.Application.Services;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Tests.Services;

public class NoteServiceTests
{
    private readonly Mock<INoteRepository> _noteRepositoryMock;
    private readonly Mock<IReminderRepository> _reminderRepositoryMock;
    private readonly NoteService _noteService;

    public NoteServiceTests()
    {
        _noteRepositoryMock = new Mock<INoteRepository>();
        _reminderRepositoryMock = new Mock<IReminderRepository>();

        _noteService = new NoteService(
            _noteRepositoryMock.Object,
            _reminderRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsUserNotes()
    {
        var notes = new List<Note>
        {
            CreateNote(1, 10, "First note"),
            CreateNote(2, 10, "Second note")
        };

        _noteRepositoryMock
            .Setup(x => x.GetAllAsync(10))
            .ReturnsAsync(notes);

        var result = await _noteService.GetAllAsync(10);

        Assert.Equal(2, result.Count);
        Assert.Equal("First note", result[0].Title);
        Assert.Equal("Second note", result[1].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNoteExists_ReturnsNote()
    {
        var note = CreateNote(1, 10, "Test note");

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        var result = await _noteService.GetByIdAsync(1, 10);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test note", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNoteDoesNotExist_ThrowsNoteNotFoundException()
    {
        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync((Note?)null);

        await Assert.ThrowsAsync<NoteNotFoundException>(
            () => _noteService.GetByIdAsync(1, 10));
    }

    [Fact]
    public async Task CreateAsync_CreatesNoteForUser()
    {
        var request = new CreateNoteRequestDto
        {
            Title = "New note",
            Content = "Note content"
        };

        Note? addedNote = null;

        _noteRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Note>()))
            .Callback<Note>(note => addedNote = note)
            .Returns(Task.CompletedTask);

        var result = await _noteService.CreateAsync(request, 10);

        Assert.NotNull(addedNote);
        Assert.Equal("New note", addedNote.Title);
        Assert.Equal("Note content", addedNote.Content);
        Assert.Equal(10, addedNote.UserId);

        Assert.Equal("New note", result.Title);
        Assert.Equal("Note content", result.Content);

        _noteRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Note>()),
            Times.Once);

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesNote()
    {
        var note = CreateNote(1, 10, "Old title");
        note.Content = "Old content";

        var request = new UpdateNoteRequestDto
        {
            Title = "New title",
            Content = "New content"
        };

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        var result = await _noteService.UpdateAsync(1, request, 10);

        Assert.Equal("New title", note.Title);
        Assert.Equal("New content", note.Content);
        Assert.Equal("New title", result.Title);
        Assert.Equal("New content", result.Content);

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task CompleteAsync_MarksNoteAsCompleted()
    {
        var note = CreateNote(1, 10, "Test note");

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        var result = await _noteService.CompleteAsync(1, 10);

        Assert.True(note.IsCompleted);
        Assert.True(result.IsComlete);

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UnCompleteAsync_MarksNoteAsNotCompleted()
    {
        var note = CreateNote(1, 10, "Test note");
        note.IsCompleted = true;

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        var result = await _noteService.UnCompleteAsync(1, 10);

        Assert.False(note.IsCompleted);
        Assert.False(result.IsComlete);

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesNote()
    {
        var note = CreateNote(1, 10, "Test note");

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        var result = await _noteService.DeleteAsync(1, 10);

        Assert.NotNull(note.DeletedAt);

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_RestoresDeletedNote()
    {
        var note = CreateNote(1, 10, "Deleted note");
        note.DeletedAt = DateTime.UtcNow;

        _noteRepositoryMock
            .Setup(x => x.GetDeletedNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        var result = await _noteService.RestoreAsync(1, 10);

        Assert.Null(note.DeletedAt);

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_WhenNoteDoesNotExist_ThrowsNoteNotFoundException()
    {
        _noteRepositoryMock
            .Setup(x => x.GetDeletedNoteByIdAsync(1, 10))
            .ReturnsAsync((Note?)null);

        await Assert.ThrowsAsync<NoteNotFoundException>(
            () => _noteService.RestoreAsync(1, 10));
    }

    [Fact]
    public async Task HardDeleteAsync_RemovesNotePermanently()
    {
        var note = CreateNote(1, 10, "Deleted note");
        note.DeletedAt = DateTime.UtcNow;

        _noteRepositoryMock
            .Setup(x => x.GetDeletedNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        await _noteService.HardDeleteAsync(1, 10);

        _noteRepositoryMock.Verify(
            x => x.RemoveNote(note),
            Times.Once);

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task AddReminderAsync_AddsReminderToNote()
    {
        var note = CreateNote(1, 10, "Test note");

        var notifyAt = DateTime.UtcNow.AddHours(1);

        var request = new CreateReminderRequestDto
        {
            NotifyAt = notifyAt
        };

        Reminder? addedReminder = null;

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        _reminderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Reminder>()))
            .Callback<Reminder>(reminder => addedReminder = reminder)
            .Returns(Task.CompletedTask);

        var result = await _noteService.AddReminderAsync(
            1,
            request,
            10);

        Assert.NotNull(addedReminder);
        Assert.Equal(1, addedReminder.NoteId);
        Assert.Equal(notifyAt, addedReminder.NotifyAt);

        Assert.Equal(1, result.NoteId);
        Assert.Equal(notifyAt, result.NotifyAt);

        _reminderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Reminder>()),
            Times.Once);

        _reminderRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateReminderAsync_UpdatesReminder()
    {
        var note = CreateNote(1, 10, "Test note");

        var reminder = new Reminder
        {
            Id = 5,
            NoteId = 1,
            NotifyAt = DateTime.UtcNow.AddHours(1),
            Note = note
        };

        note.Reminders.Add(reminder);

        var newNotifyAt = DateTime.UtcNow.AddHours(2);

        var request = new UpdateReminderRequestDto
        {
            NotifyAt = newNotifyAt
        };

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        var result = await _noteService.UpdateReminderAsync(
            1,
            5,
            request,
            10);

        Assert.Equal(newNotifyAt, reminder.NotifyAt);
        Assert.Equal(newNotifyAt, result.NotifyAt);

        _reminderRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateReminderAsync_WhenReminderDoesNotExist_ThrowsReminderNotFoundException()
    {
        var note = CreateNote(1, 10, "Test note");

        var request = new UpdateReminderRequestDto
        {
            NotifyAt = DateTime.UtcNow.AddHours(1)
        };

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        await Assert.ThrowsAsync<ReminderNotFoundException>(
            () => _noteService.UpdateReminderAsync(
                1,
                999,
                request,
                10));
    }

    [Fact]
    public async Task DeleteReminderAsync_RemovesReminder()
    {
        var note = CreateNote(1, 10, "Test note");

        var reminder = new Reminder
        {
            Id = 5,
            NoteId = 1,
            NotifyAt = DateTime.UtcNow.AddHours(1),
            Note = note
        };

        note.Reminders.Add(reminder);

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        await _noteService.DeleteReminderAsync(
            1,
            5,
            10);

        _reminderRepositoryMock.Verify(
            x => x.Remove(reminder),
            Times.Once);

        _reminderRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DeleteReminderAsync_WhenReminderDoesNotExist_ThrowsReminderNotFoundException()
    {
        var note = CreateNote(1, 10, "Test note");

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 10))
            .ReturnsAsync(note);

        await Assert.ThrowsAsync<ReminderNotFoundException>(
            () => _noteService.DeleteReminderAsync(
                1,
                999,
                10));
    }
    [Fact]
    public async Task GetByIdAsync_WhenRepositoryReturnsNullForUser_ThrowsNoteNotFoundException()
    {
        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 20))
            .ReturnsAsync((Note?)null);

        await Assert.ThrowsAsync<NoteNotFoundException>(
            () => _noteService.GetByIdAsync(1, 20));

        _noteRepositoryMock.Verify(
            x => x.GetNoteByIdAsync(1, 20),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNoteDoesNotBelongToUser_ThrowsNoteNotFoundException()
    {
        var request = new UpdateNoteRequestDto
        {
            Title = "Changed title",
            Content = "Changed content"
        };

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 20))
            .ReturnsAsync((Note?)null);

        await Assert.ThrowsAsync<NoteNotFoundException>(
            () => _noteService.UpdateAsync(1, request, 20));

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenNoteDoesNotBelongToUser_ThrowsNoteNotFoundException()
    {
        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 20))
            .ReturnsAsync((Note?)null);

        await Assert.ThrowsAsync<NoteNotFoundException>(
            () => _noteService.DeleteAsync(1, 20));

        _noteRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }
    
    [Fact]
    public async Task AddReminderAsync_WhenNoteDoesNotBelongToUser_ThrowsNoteNotFoundException()
    {
        var request = new CreateReminderRequestDto
        {
            NotifyAt = DateTime.UtcNow.AddHours(1)
        };

        _noteRepositoryMock
            .Setup(x => x.GetNoteByIdAsync(1, 20))
            .ReturnsAsync((Note?)null);

        await Assert.ThrowsAsync<NoteNotFoundException>(
            () => _noteService.AddReminderAsync(
                1,
                request,
                20));

        _reminderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Reminder>()),
            Times.Never);
    }
    private static Note CreateNote(
        int id,
        int userId,
        string title)
    {
        return new Note
        {
            Id = id,
            UserId = userId,
            Title = title,
            Content = "Test content"
        };
    }
}