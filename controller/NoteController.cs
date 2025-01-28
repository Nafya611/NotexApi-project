using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;
    private readonly IConfiguration _config;

    public NoteController(INoteService noteService, IConfiguration config)
    {
        _config = config;
        _noteService = noteService;
    }

    // CREATE: api/note
    [HttpPost]
    public async Task<IActionResult> CreateNoteAsync([FromBody] Note note)
    {
        if (note == null || string.IsNullOrEmpty(note.Title) || string.IsNullOrEmpty(note.Content))
        {
            return BadRequest(new { message = "Note title and content are required." });
        }

        var createdNote = await _noteService.CreateNoteAsync(note);
        return CreatedAtAction(nameof(GetNoteByIdAsync), new { id = createdNote.Id }, createdNote);
    }

    // READ: api/note/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNoteByIdAsync(string id)
    {
        var note = await _noteService.GetNoteByIdAsync(id);

        if (note == null)
        {
            return NotFound(new { message = "Note not found." });
        }

        return Ok(note);
    }

    // READ ALL: api/note
    [HttpGet]
    public async Task<IActionResult> GetAllNotesAsync()
    {
        var notes = await _noteService.GetAllNotesAsync();
        return Ok(notes);
    }

    // UPDATE: api/note/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNoteAsync(string id, [FromBody] Note note)
    {
        if (note == null || string.IsNullOrEmpty(note.Title) || string.IsNullOrEmpty(note.Content))
        {
            return BadRequest(new { message = "Note title and content are required." });
        }

        var updatedNote = await _noteService.UpdateNoteAsync(id, note);

        if (updatedNote == null)
        {
            return NotFound(new { message = "Note not found." });
        }

        return Ok(updatedNote);
    }

    // DELETE: api/note/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNoteAsync(string id)
    {
        var isDeleted = await _noteService.DeleteNoteAsync(id);

        if (!isDeleted)
        {
            return NotFound(new { message = "Note not found." });
        }

        return NoContent(); // Status 204, meaning the resource was deleted successfully
    }
}
