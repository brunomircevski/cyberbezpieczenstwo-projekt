using Microsoft.EntityFrameworkCore;
using BDwAS_projekt.Models;

namespace BDwAS_projekt.Data;

public class SQLiteContext(DbContextOptions<SQLiteContext> options) : DbContext(options)//, IDbContext
{
    private DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
