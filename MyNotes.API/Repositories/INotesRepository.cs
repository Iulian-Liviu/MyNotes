using MyNotes.API.Models.DtoModels;

namespace MyNotes.API.Repositories
{
    public interface INotesRepository : IDisposable
    {
        public Task<IEnumerable<NoteResponse>> GetAllNotes(CancellationToken token);
        public Task<bool> CreateNote(NoteUpload note, CancellationToken token);
        public Task<bool> DeleteNote(Guid noteId, CancellationToken token);
        public Task<bool> UpdateNote(Guid noteId, NoteUpload noteUpload, CancellationToken token);


    }
}
