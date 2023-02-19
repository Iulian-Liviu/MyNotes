using System.Text.Json.Serialization;

namespace MyNotes.Desktop.Models
{
    public class NoteUpload
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("body")]

        public string? Body { get; set; }
    }
}
