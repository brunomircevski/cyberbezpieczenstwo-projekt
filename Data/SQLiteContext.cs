using Microsoft.EntityFrameworkCore;
using BDwAS_projekt.Models;

namespace BDwAS_projekt.Data;

public class SQLiteContext(DbContextOptions<SQLiteContext> options) : DbContext(options), IDbContext
{
    private DbSet<User> Users { get; set; }

    public void AddUsers(User user)
    {
        throw new NotImplementedException();
    }

    public void DeleteUser(string id)
    {
        throw new NotImplementedException();
    }

    public List<Channel> GetChannels()
    {
        throw new NotImplementedException();
    }

    public User GetUser(string id)
    {
        throw new NotImplementedException();
    }

    public List<User> GetUsers()
    {
        throw new NotImplementedException();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
