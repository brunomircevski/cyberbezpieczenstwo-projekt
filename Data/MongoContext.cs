using BDwAS_projekt.Models;
using MongoDB.Bson;
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

    private IMongoCollection<User> Users => _database.GetCollection<User>("Users");

    public List<User> GetUsers()
    {
        try
        {
            var projection = Builders<User>.Projection.Exclude(u => u.Subscriptions);
            var users = Users.Find(_ => true).Project<User>(projection).ToList();

            return users;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public User GetUser(string id)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var user = Users.Find(filter).FirstOrDefault();
            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool AddUser(User user)
    {
        Users.InsertOne(user);
        return true;
    }

    public bool UpdateUser(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);

        var updates = new List<UpdateDefinition<User>>();

        if (user.FirstName != null)
            updates.Add(Builders<User>.Update.Set(u => u.FirstName, user.FirstName));
        if (user.LastName != null)
            updates.Add(Builders<User>.Update.Set(u => u.LastName, user.LastName));
        if (user.Email != null)
            updates.Add(Builders<User>.Update.Set(u => u.Email, user.Email));
            
        var updateDefinition = Builders<User>.Update.Combine(updates);

        var result = Users.UpdateOne(filter, updateDefinition);

        return result.ModifiedCount > 0;
    }

    public bool VerifyUser(string userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var update = Builders<User>.Update.Set(u => u.IsVerified, true);

        var result = Users.UpdateOne(filter, update);

        return result.ModifiedCount > 0;
    }

    public bool DeleteUser(string userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var result = Users.DeleteOne(filter);

        return result.DeletedCount > 0;
    }

    public bool AddSubscription(Subscription subscription, string userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var update = Builders<User>.Update.Push(u => u.Subscriptions, subscription);

        var result = Users.UpdateOne(filter, update);

        return result.ModifiedCount > 0;
    }

    public bool AddPayment(Payment payment, string subscriptionId, string userId)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(u => u.Id, userId),
            Builders<User>.Filter.ElemMatch(u => u.Subscriptions, s => s.Id == subscriptionId)
        );
        var update = Builders<User>.Update.Push("Subscriptions.$.Payments", payment);

        var result = Users.UpdateOne(filter, update);

        return result.ModifiedCount > 0;
    }

    public List<Channel> GetChannels()
    {
        throw new NotImplementedException();
    }
}
