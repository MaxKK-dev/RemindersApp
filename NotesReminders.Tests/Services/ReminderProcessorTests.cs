using Moq;

using NotesReminders.Application.Interfaces;
using NotesReminders.Application.Services;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Tests.Services;

public class ReminderProcessorTests
{
    private readonly Mock<IReminderRepository> _reminderRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly ReminderProcessor _processor;

    public ReminderProcessorTests()
    {
        _reminderRepositoryMock = new Mock<IReminderRepository>();
        _notificationServiceMock = new Mock<INotificationService>();

        _processor = new ReminderProcessor(
            _reminderRepositoryMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task ProcessAsync_WhenReminderIsDue_SendsNotification()
    {
        var reminder = CreateReminder();

        _reminderRepositoryMock
            .Setup(x => x.GetDueRemindersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Reminder> { reminder });

        await _processor.ProcessAsync(CancellationToken.None);

        _notificationServiceMock.Verify(
            x => x.SendAsync(reminder),
            Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_WhenThereAreNoDueReminders_DoesNotSendNotification()
    {
        _reminderRepositoryMock
            .Setup(x => x.GetDueRemindersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Reminder>());

        await _processor.ProcessAsync(CancellationToken.None);

        _notificationServiceMock.Verify(
            x => x.SendAsync(It.IsAny<Reminder>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_WhenMultipleRemindersAreDue_SendsNotificationForEach()
    {
        var reminder1 = CreateReminder();
        var reminder2 = CreateReminder();
        var reminder3 = CreateReminder();

        var reminders = new List<Reminder>
        {
            reminder1,
            reminder2,
            reminder3
        };

        _reminderRepositoryMock
            .Setup(x => x.GetDueRemindersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(reminders);

        await _processor.ProcessAsync(CancellationToken.None);

        _notificationServiceMock.Verify(
            x => x.SendAsync(It.IsAny<Reminder>()),
            Times.Exactly(3));

        _notificationServiceMock.Verify(
            x => x.SendAsync(reminder1),
            Times.Once);

        _notificationServiceMock.Verify(
            x => x.SendAsync(reminder2),
            Times.Once);

        _notificationServiceMock.Verify(
            x => x.SendAsync(reminder3),
            Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_WhenRemindersAreProcessed_RemovesThem()
    {
        var reminder1 = CreateReminder();
        var reminder2 = CreateReminder();

        var reminders = new List<Reminder>
        {
            reminder1,
            reminder2
        };

        _reminderRepositoryMock
            .Setup(x => x.GetDueRemindersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(reminders);

        await _processor.ProcessAsync(CancellationToken.None);

        _reminderRepositoryMock.Verify(
            x => x.RemoveRange(
                It.Is<IEnumerable<Reminder>>(r =>
                    r.SequenceEqual(reminders))),
            Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_WhenRemindersAreProcessed_SavesChanges()
    {
        _reminderRepositoryMock
            .Setup(x => x.GetDueRemindersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Reminder>());

        await _processor.ProcessAsync(CancellationToken.None);

        _reminderRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_WhenCancellationIsRequested_ThrowsOperationCanceledException()
    {
        var reminder = CreateReminder();

        _reminderRepositoryMock
            .Setup(x => x.GetDueRemindersAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<Reminder> { reminder });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _processor.ProcessAsync(
                cancellationTokenSource.Token));

        _notificationServiceMock.Verify(
            x => x.SendAsync(It.IsAny<Reminder>()),
            Times.Never);
    }

    private static Reminder CreateReminder()
    {
        return new Reminder
        {
            Id = 1,
            NoteId = 1,
            NotifyAt = DateTime.UtcNow.AddMinutes(-1)
        };
    }
}