using Microsoft.AspNetCore.Mvc;
using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;
using MyNotes.API.Repositories;

namespace MyNotes.API.Controllers
{
    [Route("/api/notes")]
    [ApiController]
    public class NotesController : Controller
    {
        private INotesRepository _notesRepository;
        public NotesController(INotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotes(CancellationToken token)
        {
            try
            {
                return Ok(await _notesRepository.GetAllNotes(token));

            }

            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }

            catch (Exception e)
            {

                return Problem(detail: e.Message, "MyNotes.API.Database -> " + e.Source + " -> " + e?.TargetSite?.Name ?? "UNKNOWN METHOD", 500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertNote([FromBody] NoteUpload noteUpload, CancellationToken token)
        {
            try
            {
                if (!string.IsNullOrEmpty(noteUpload.Title))
                {
                    if (await _notesRepository.CreateNote(noteUpload, token))
                        return Ok();
                    else
                        return BadRequest(new BadInputResponse("The specified body was empty or an bad JSON string."));
                }
                else
                {
                    return BadRequest(new BadInputResponse("At least the title property should be an actual value not empty."));
                }
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, "MyNotes.API.Database -> " + e.Source + " -> " + e?.TargetSite?.Name ?? "UNKNOWN METHOD", 500, "Internal Server Error");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNote([FromQuery] string noteId, CancellationToken token)
        {
            try
            {
                noteId = noteId.Replace("\"", "");
                if (await _notesRepository.DeleteNote(Guid.Parse(noteId), token))
                    return NoContent();
                else
                    return BadRequest(new BadInputResponse("There is no note in the database with the specified ID."));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
            catch (FormatException)
            {
                return BadRequest(new BadInputResponse("The specified parameter is not a valid GUID string."));
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, "MyNotes.API.Database -> " + e.Source + " -> " + e?.TargetSite?.Name ?? "UNKNOWN METHOD", 500, "Internal Server Error");
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateNote([FromQuery] string noteId, [FromBody] NoteUpload noteUpload, CancellationToken token)
        {
            try
            {
                noteId = noteId.Replace("\"", "");
                if (!string.IsNullOrEmpty(noteUpload.Title))
                {
                    if (await _notesRepository.UpdateNote(Guid.Parse(noteId), noteUpload, token))
                        return NoContent();
                    else
                        return BadRequest(new BadInputResponse("There is no note in the database with the specified ID."));
                }
                else
                {
                    return BadRequest(new BadInputResponse("At least the title property should be an actual value not empty."));

                }
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
            catch (FormatException)
            {
                return BadRequest(new BadInputResponse("The specified parameter is not a valid GUID string."));
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, "MyNotes.API.Database -> " + e.Source + " -> " + e?.TargetSite?.Name ?? "UNKNOWN METHOD", 500, "Internal Server Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _notesRepository.Dispose();
        }
    }
}
