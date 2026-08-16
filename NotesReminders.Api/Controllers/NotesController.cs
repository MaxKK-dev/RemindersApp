using NotesReminders.Application.Interfaces;
using NotesReminders.Application.DTOs.Note;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NotesReminders.Api.Extensions;

namespace NotesReminders.Api.Controllers;

[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;
    public NotesController (INoteService noteService)
    {
        _noteService = noteService;
    }
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetNoteById(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.GetByIdAsync(id, userId));
    }
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyCollection<NoteResponseDto>>> GetAllNotesAsync()
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.GetAllAsync(userId));
    }
    [HttpGet("deleted")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyCollection<NoteResponseDto>>> GetAllDeletedNotesAsync()
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.GetAllDeletedAsync(userId));
    }
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> CreateNoteAsync([FromBody] CreateNoteRequestDto request)
    {
        var userId = User.GetUserId();
        var result = await _noteService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetNoteById), new {id = result.Id}, result);
    }
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> UpdateNoteAsync([FromBody] UpdateNoteRequestDto request, int id)
    {
        var userId = User.GetUserId();
        var result = await _noteService.UpdateAsync(id, request, userId);
        return Ok(result);
    }
    [HttpPatch("{id:int}/complete")]    
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> CompleteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.CompleteAsync(id, userId));
    }
    [HttpPatch("{id:int}/uncomplete")]
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> UncompleteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.UnCompleteAsync(id, userId));
    }
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> DeleteNoteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.DeleteAsync(id, userId));
    }
    [HttpPatch("{id:int}/restore")]
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> RestoreNoteAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(await _noteService.RestoreAsync(id, userId));
    }
    [HttpDelete("{id:int}/permanent")]
    [Authorize]
    public async Task<ActionResult> HardDeleteAsync(int id)
    {
        var userId = User.GetUserId();
        await _noteService.HardDeleteAsync(id, userId);
        return NoContent();
    }
    
}
