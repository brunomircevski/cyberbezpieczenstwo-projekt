using BDwAS_projekt.Models;
using MongoDB.Driver;

namespace BDwAS_projekt.Data;

public class MongoContext() : IDbContext
{
    private readonly IMongoDatabase _database;
    public MongoContext(string connectionString, string dbName) : this()
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(dbName);
    }

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
}
