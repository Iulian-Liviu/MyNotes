using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNotes.API.Database;
using MyNotes.API.Models;
using MyNotes.API.Models.DtoModels;

namespace MyNotes.API.Repositories
{
    public class NotesRepository : INotesRepository
    {
        private protected readonly MyNotesDbContext _context;
        private protected readonly IMapper _mapper;
        private bool disposedValue;

        public NotesRepository(MyNotesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateNote(NoteUpload note, CancellationToken token)
        {
            var storeNote = _mapper.Map<NoteUpload, Note>(note);
            await _context.Notes.AddAsync(storeNote, token);
            if (await _context.SaveChangesAsync(token) > 0) return true;
            return false;
        }

        public async Task<IEnumerable<NoteResponse>> GetAllNotes(CancellationToken token)
        {
            return await _context.Notes.Select(s => _mapper.Map<NoteResponse>(s)).ToListAsync(token);
        }

        public async Task<bool> DeleteNote(Guid noteId, CancellationToken token)
        {
            var note = await _context.Notes
                .Where(n => n.NoteId == noteId)
                .FirstOrDefaultAsync(token);
            if (note is null)
            {
                return false;
            }
            _context.Notes.Remove(note);
            return await _context.SaveChangesAsync(token) > 0;
        }

        [HttpPut]
        public async Task<bool> UpdateNote(Guid noteId, NoteUpload noteUpload, CancellationToken token)
        {
            var note = await _context.Notes
                        .Where(n => n.NoteId == noteId)
                        .FirstOrDefaultAsync(token);
            if (note is null)
            {
                return false;
            }

            note.Title = noteUpload.Title;
            note.Body = noteUpload.Body;
            note.ModifiedAt = DateTime.UtcNow;
            _context.Notes.Update(note);
            return await _context.SaveChangesAsync(token) > 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        /*        ~NotesRepository()
        {
            this.Dispose();
        }*/
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
