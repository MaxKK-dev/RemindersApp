using NotesReminders.Application.Interfaces;
using NotesReminders.Application.DTOs.Note;
using NotesReminders.Application.DTOs.Reminder;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NotesReminders.Api.Extensions;

namespace NotesReminders.Api.Controllers;

[ApiController]
[Route("api/notes")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;
    public NotesController (INoteService noteService)
    {
        _noteService = noteService;
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetNoteById(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.GetByIdAsync(id, userId));
    }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<NoteResponseDto>>> GetAllNotesAsync()
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.GetAllAsync(userId));
    }
    [HttpGet("deleted")]
    public async Task<ActionResult<IReadOnlyCollection<NoteResponseDto>>> GetAllDeletedNotesAsync()
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.GetAllDeletedAsync(userId));
    }
    [HttpPost]
    public async Task<ActionResult<NoteResponseDto>> CreateNoteAsync([FromBody] CreateNoteRequestDto request)
    {
        var userId = User.GetUserId();
        var result = await _noteService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetNoteById), new {id = result.Id}, result);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<NoteResponseDto>> UpdateNoteAsync([FromBody] UpdateNoteRequestDto request, int id)
    {
        var userId = User.GetUserId();
        var result = await _noteService.UpdateAsync(id, request, userId);
        return Ok(result);
    }
    [HttpPatch("{id:int}/complete")]    
    public async Task<ActionResult<NoteResponseDto>> CompleteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.CompleteAsync(id, userId));
    }
    [HttpPatch("{id:int}/uncomplete")]
    public async Task<ActionResult<NoteResponseDto>> UncompleteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.UnCompleteAsync(id, userId));
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<NoteResponseDto>> DeleteNoteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.DeleteAsync(id, userId));
    }
    [HttpPatch("{id:int}/restore")]
    public async Task<ActionResult<NoteResponseDto>> RestoreNoteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.RestoreAsync(id, userId));
    }
    [HttpDelete("{id:int}/permanent")]
    public async Task<ActionResult> HardDeleteAsync(int id)
    {
        var userId = User.GetUserId();
        await _noteService.HardDeleteAsync(id, userId);
        return NoContent();
    }
    // Remindr operation

    [HttpPost("{noteId:int}/reminders")]
    public async Task<ActionResult<ReminderResponseDto>> AddReminderAsync(int noteId,
        [FromBody] CreateReminderRequestDto request)
    {
        var userId = User.GetUserId();

        var reminder = await _noteService.AddReminderAsync(noteId, request, userId);

        return CreatedAtAction(nameof(GetNoteById), new { id = noteId }, reminder);
    }

    [HttpPut("{noteId:int}/reminders/{reminderId:int}")]
    public async Task<ActionResult<ReminderResponseDto>> UpdateReminderAsync(int noteId,
        int reminderId,[FromBody] UpdateReminderRequestDto request)
    {
        var userId = User.GetUserId();
        var reminder = await _noteService.UpdateReminderAsync(noteId, reminderId, request, userId);

        return Ok(reminder);
    }

    [HttpDelete("{noteId:int}/reminders/{reminderId:int}")]
    public async Task<IActionResult> DeleteReminderAsync(int noteId, int reminderId)
    {
        var userId = User.GetUserId();
        await _noteService.DeleteReminderAsync(noteId, reminderId, userId);

        return NoContent();
    }   
}
