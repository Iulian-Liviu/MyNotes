using Microsoft.EntityFrameworkCore;
using MyNotes.API.Models;

namespace MyNotes.API.Database;

public class MyNotesDbContext : DbContext
{
    public MyNotesDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Note> Notes { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<ApiKey> Keys { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>()
            .HasKey(object? (x) => x.Id);
        modelBuilder.Entity<Note>()
            .Property(string? (n) => n.Title)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Note>()
            .HasOne(User? (n) => n.User)
            .WithMany(IEnumerable<Note>? (n) => n.Notes);

        modelBuilder.Entity<User>()
            .HasKey(object? (u) => u.UserId);

        modelBuilder.Entity<User>()
            .HasOne<ApiKey>(u => u.ApiKey)
            .WithOne(User? (a) => a.User)
            .HasForeignKey<ApiKey>(object? (a) => a.KeyId);

        modelBuilder.Entity<ApiKey>()
            .HasKey(a => a.KeyId);

        base.OnModelCreating(modelBuilder);
    }
}