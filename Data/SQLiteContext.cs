using Microsoft.EntityFrameworkCore;
using Cyberbezpieczenstwo.Models;

namespace Cyberbezpieczenstwo.Data;

public class SQLiteContext: DbContext
{
    private readonly ILogger<SQLiteContext> _logger;

    private DbSet<Message> Messages { get; set; }
    private DbSet<User> Users { get; set; }

    public SQLiteContext(DbContextOptions<SQLiteContext> options, ILogger<SQLiteContext> logger) : base(options)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

}
