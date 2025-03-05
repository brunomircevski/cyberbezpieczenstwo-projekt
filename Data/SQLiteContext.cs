using Microsoft.EntityFrameworkCore;
using Cyberbezpieczenstwo.Models;

namespace Cyberbezpieczenstwo.Data;

public class SQLiteContext(DbContextOptions<SQLiteContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the one-to-many relationship for OwnMessages (User -> Message)
        modelBuilder.Entity<User>()
            .HasMany(u => u.OwnMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId);

        // Configure the many-to-many relationship for EditableMessages (User <-> Message)
        modelBuilder.Entity<Message>()
            .HasMany(m => m.Editors)
            .WithMany(u => u.EditableMessages)
            .UsingEntity(j => j.ToTable("MessageEditors"));

        base.OnModelCreating(modelBuilder);
    }

}
