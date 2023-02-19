using Microsoft.EntityFrameworkCore;
using MyNotes.API.Models;

namespace MyNotes.API.Database
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Note> Notes { get; set; }
    }
}
