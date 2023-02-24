namespace MyNotes.API.Models.DtoModels;

public class NoteResponse
{
    public Guid NoteId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}