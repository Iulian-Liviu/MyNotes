using System;
using System.Text.Json.Serialization;

namespace MyNotes.Desktop.Models
{
    public class Note
    {
        [JsonPropertyName("noteId")]
        public Guid NoteId { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("body")]

        public string? Body { get; set; }
        [JsonPropertyName("addedAt")]

        public DateTime AddedAt { get; set; }
        [JsonPropertyName("modifiedAt")]
        public DateTime ModifiedAt { get; set; }
    }
}
