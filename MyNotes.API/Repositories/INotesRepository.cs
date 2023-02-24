using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;

namespace MyNotes.API.Repositories;

public interface INotesRepository : IDisposable
{
    public Task<ResultResponse<IEnumerable<NoteResponse>>> GetAllNotes(string? apikey, CancellationToken token);
    public Task<ResultResponse<bool>> CreateNote(string apikey, NoteUpload note, CancellationToken token);
    public Task<ResultResponse<bool>> DeleteNote(string apikey, Guid noteId, CancellationToken token);

    public Task<ResultResponse<bool>> UpdateNote(string apikey, Guid noteId, NoteUpload noteUpload,
        CancellationToken token);
}