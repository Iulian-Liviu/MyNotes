namespace MyNotes.API.Models.DtoModels;

public class UserResponse
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public ApiKey? ApiKey { get; set; }
}