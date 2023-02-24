namespace MyNotes.API.Models;

public class Note
{
    public int Id { get; set; }
    public Guid NoteId { get; set; } = Guid.NewGuid();
    public string? Title { get; set; }
    public string? Body { get; set; }
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public virtual User? User { get; set; }
}