using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteManagementSystem.Models;
using NoteManagementSystem.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoteManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly NoteService _noteService;

        public NotesController(NoteService noteService)
        {
            _noteService = noteService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet]
        public async Task<ActionResult<List<Note>>> Get()
        {
            var notes = await _noteService.GetAsync(GetUserId());
            if (notes == null || notes.Count == 0)
            {
                return NotFound(new { Message = "No notes found." });
            }
            return Ok(notes);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Note>> Get(string id)
        {
            var note = await _noteService.GetAsync(id, GetUserId());
            if (note == null)
            {
                return NotFound(new { Message = "Note not found." });
            }
            return Ok(note);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Note>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new { Message = "Search term cannot be empty." });
            }

            var notes = await _noteService.SearchAsync(GetUserId(), term);
            if (notes == null || notes.Count == 0)
            {
                return NotFound(new { Message = "No notes found matching the search term." });
            }

            return Ok(notes);
        }

        [HttpGet("category/{categoryId:length(24)}")]
        public async Task<ActionResult<List<Note>>> GetByCategory(string categoryId)
        {
            var notes = await _noteService.GetByCategoryAsync(categoryId, GetUserId());
            if (notes == null || notes.Count == 0)
            {
                return NotFound(new { Message = "No notes found for the specified category." });
            }
            return Ok(notes);
        }

        [HttpPost]
        public async Task<ActionResult<Note>> Create(Note note)
        {
            note.UserId = GetUserId();
            await _noteService.CreateAsync(note);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Note noteIn)
        {
            var note = await _noteService.GetAsync(id, GetUserId());
            if (note == null)
            {
                return NotFound(new { Message = "Note not found." });
            }

            noteIn.Id = note.Id;
            noteIn.UserId = GetUserId();

            await _noteService.UpdateAsync(id, noteIn, GetUserId());
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var note = await _noteService.GetAsync(id, GetUserId());
            if (note == null)
            {
                return NotFound(new { Message = "Note not found." });
            }

            await _noteService.RemoveAsync(id, GetUserId());
            return NoContent();
        }
    }
}
