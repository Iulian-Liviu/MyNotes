namespace MyNotes.API.Models.ErrorResponse;

public class BadInputResponse
{
    public BadInputResponse(string message)
    {
        Message = message;
    }

    public string? Message { get; set; }
}