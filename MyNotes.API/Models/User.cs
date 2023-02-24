namespace MyNotes.API.Models;

public class User
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? UserName { get; set; }
    public virtual ApiKey? ApiKey { get; }
    public virtual ICollection<Note>? Notes { get; }
}