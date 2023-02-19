using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNotes.API.Models
{
    [Table("Notes")]
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public Guid NoteId { get; set; } = Guid.NewGuid();

        [MinLength(1)]
        [MaxLength(100)]
        public string? Title { get; set; }
        public string? Body { get; set; }
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    }
}
