using Microsoft.AspNetCore.Mvc;
using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;
using MyNotes.API.Repositories;

namespace MyNotes.API.Controllers;

[Route("/api/notes")]
[ApiController]
public class NotesController : Controller {
    private readonly INotesRepository _notesRepository;

    public NotesController(INotesRepository notesRepository) {
        _notesRepository = notesRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotes([FromQuery] string apikey, CancellationToken token) {
        try {
            var response = await _notesRepository.GetAllNotes(apikey, token);
            if (response.StatusType == StatusType.Success) return Ok(response);

            return NotFound(response);
        }

        catch (OperationCanceledException) {
            return StatusCode(499);
        }

        catch (Exception e) {
            return Problem(e.Message,
                $"MyNotes.API.Database -> {e.Source} -> {e?.TargetSite?.Name}" ?? "UNKNOWN METHOD", 500,
                "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> InsertNote([FromQuery] string apikey, [FromBody] NoteUpload noteUpload,
        CancellationToken token) {
        try {
            if (string.IsNullOrEmpty(noteUpload.Title))
                return BadRequest(
                    new BadInputResponse("At least the title property should be an actual value not empty."));

            var response = await _notesRepository.CreateNote(apikey, noteUpload, token);
            if (response.StatusType == StatusType.Success)
                return Ok(response);

            return BadRequest(response);

        }
        catch (OperationCanceledException) {
            return StatusCode(499);
        }
        catch (Exception e) {
            return Problem(e.Message,
                $"MyNotes.API.Database -> {e.Source} -> {e?.TargetSite?.Name}" ?? "UNKNOWN METHOD", 500,
                "Internal Server Error");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteNote([FromQuery] string apikey, [FromQuery] string noteId,
        CancellationToken token) {
        try {
            noteId = noteId.Replace("\"", "");
            var response = await _notesRepository.DeleteNote(apikey, Guid.Parse(noteId), token);
            if (response.StatusType == StatusType.Success)
                return Ok(response);
            return BadRequest(response);
        }
        catch (OperationCanceledException) {
            return StatusCode(499);
        }
        catch (FormatException) {
            return BadRequest(new BadInputResponse("The specified parameter is not a valid GUID string."));
        }
        catch (Exception e) {
            return Problem(e.Message,
                "MyNotes.API.Database -> " + e.Source + " -> " + e?.TargetSite?.Name ?? "UNKNOWN METHOD", 500,
                "Internal Server Error");
        }
    }


    [HttpPut]
    public async Task<IActionResult> UpdateNote([FromQuery] string apikey, [FromQuery] string noteId,
        [FromBody] NoteUpload noteUpload, CancellationToken token) {
        try {
            noteId = noteId.Replace("\"", "");

            var response = await _notesRepository.UpdateNote(apikey, Guid.Parse(noteId), noteUpload, token);
            if (response.StatusType == StatusType.Success)
                return Ok(response);
            else if (response.StatusType == StatusType.NotFound)
                return NotFound(response);

            return BadRequest(response);


        }
        catch (OperationCanceledException) {
            return StatusCode(499);
        }
        catch (FormatException) {
            return BadRequest(new BadInputResponse("The specified parameter is not a valid GUID string."));
        }
        catch (Exception e) {
            return Problem(e.Message,
                "MyNotes.API.Database -> " + e.Source + " -> " + e?.TargetSite?.Name ?? "UNKNOWN METHOD", 500,
                "Internal Server Error");
        }
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        _notesRepository.Dispose();
    }
}