using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNotes.API.Database;
using MyNotes.API.Models;
using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;

namespace MyNotes.API.Repositories;

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

    public async Task<ResultResponse<bool>> CreateNote(string apikey, NoteUpload note, CancellationToken token)
    {
        var storeNote = _mapper.Map<NoteUpload, Note>(note);
        var userKey = await _context.Keys.Where(a => a.Key == apikey).FirstOrDefaultAsync(token);
        if (userKey != null)
        {
            var actualUser = await _context.Users.Include(u => u.ApiKey).Where(u => u.UserId == userKey.UserId)
                .FirstOrDefaultAsync(token);

            storeNote.UserId = actualUser!.UserId;
            storeNote.User = actualUser;

            await _context.Notes.AddAsync(storeNote, token);

            if (await _context.SaveChangesAsync(token) > 0)
                return new ResultResponse<bool>
                {
                    StatusCode = 200,
                    StatusType = StatusType.Success
                };
        }

        return new ResultResponse<bool>
        {
            StatusCode = 404,
            StatusType = StatusType.NotFound
        };
    }

    public async Task<ResultResponse<IEnumerable<NoteResponse>>> GetAllNotes(string? apikey, CancellationToken token)
    {
        var userKey = await _context.Keys.Where(a => a.Key == apikey).FirstOrDefaultAsync(token);

        if (userKey != null)
        {
            var userNotes = await _context.Users.Include(u => u.Notes).Where(u => u.UserId == userKey.UserId)
                .FirstOrDefaultAsync(token);
            return new ResultResponse<IEnumerable<NoteResponse>>
            {
                Data = userNotes?.Notes?.Select(n => _mapper.Map<NoteResponse>(n)).ToList(),
                StatusCode = 200,
                StatusType = StatusType.Success
            };
        }

        return new ResultResponse<IEnumerable<NoteResponse>>
        {
            StatusCode = 404,
            StatusType = StatusType.NotFound
        };
    }

    public async Task<ResultResponse<bool>> DeleteNote(string apikey, Guid noteId, CancellationToken token)
    {
        var note = await _context.Notes
            .Where(n => n.NoteId == noteId)
            .FirstOrDefaultAsync(token);
        if (note is null)
            return new ResultResponse<bool>
            {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };
        _context.Notes.Remove(note);
        await _context.SaveChangesAsync(token);

        return new ResultResponse<bool>
        {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }

    [HttpPut]
    public async Task<ResultResponse<bool>> UpdateNote(string apikey, Guid noteId, NoteUpload noteUpload,
        CancellationToken token)
    {
        var note = await _context.Notes
            .Where(n => n.NoteId == noteId)
            .FirstOrDefaultAsync(token);
        if (note is null)
            return new ResultResponse<bool>
            {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };

        note.Title = noteUpload.Title;
        note.Body = noteUpload.Body;
        note.ModifiedAt = DateTime.UtcNow;
        _context.Notes.Update(note);
        await _context.SaveChangesAsync(token);
        return new ResultResponse<bool>
        {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }

    /*        ~NotesRepository()
    {
        this.Dispose();
    }*/
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing) _context.Dispose();

            disposedValue = true;
        }
    }
}