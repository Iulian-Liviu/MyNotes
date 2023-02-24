using Newtonsoft.Json;

namespace MyNotes.API.Models;

public class ApiKey
{
    public int KeyId { get; set; }
    public string? Key { get; set; }

    [JsonIgnore] public int UserId { get; set; }

    [JsonIgnore] public virtual User? User { get; set; }
}