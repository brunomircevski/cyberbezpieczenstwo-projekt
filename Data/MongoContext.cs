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
    private IMongoCollection<Channel> Channels => _database.GetCollection<Channel>("Channels");
    private IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");

    public List<User> GetUsers()
    {
        try
        {
            var projection = Builders<User>.Projection.Exclude(u => u.Subscriptions);
            var users = Users.Find(_ => true).Project<User>(projection).ToList();

            return users;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
    }

    public User GetUser(string userId)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var user = Users.Find(filter).FirstOrDefault();
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
    }

    public bool AddUser(User user)
    {
        try
        {
            Users.InsertOne(user);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool UpdateUser(User user)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool VerifyUser(string userId)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.IsVerified, true);

            var result = Users.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteUser(string userId)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var result = Users.DeleteOne(filter);

            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddSubscription(Subscription subscription, string userId)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Push(u => u.Subscriptions, subscription);

            var result = Users.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddPayment(Payment payment, string subscriptionId, string userId)
    {
        try
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, userId),
                Builders<User>.Filter.ElemMatch(u => u.Subscriptions, s => s.Id == subscriptionId)
            );
            var update = Builders<User>.Update.Push("Subscriptions.$.Payments", payment);

            var result = Users.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public List<Channel> GetChannels()
    {
        try
        {
            var projection = Builders<Channel>.Projection
                .Exclude(c => c.Streams)
                .Exclude(c => c.Posts)
                .Exclude(c => c.Plans);
            var channels = Channels.Find(_ => true).Project<Channel>(projection).ToList();

            return channels;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
    }

    public Channel GetChannel(string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var projection = Builders<Channel>.Projection
                .Exclude("Posts.Comments")
                .Exclude("Posts.Ratings")
                .Exclude("Posts.Images")
                .Exclude("Posts.Attachments");
            var channel = Channels.Find(filter).Project<Channel>(projection).FirstOrDefault();

            return channel;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
    }

    public bool AddChannel(Channel channel)
    {
        try
        {
            Channels.InsertOne(channel);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool UpdateChannel(Channel channel)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(u => u.Id, channel.Id);

            var updates = new List<UpdateDefinition<Channel>>();

            if (channel.Name != null)
                updates.Add(Builders<Channel>.Update.Set(c => c.Name, channel.Name));
            if (channel.Description != null)
                updates.Add(Builders<Channel>.Update.Set(c => c.Description, channel.Description));
            if (channel.OwnerId != null)
                updates.Add(Builders<Channel>.Update.Set(c => c.OwnerId, channel.OwnerId));

            var updateDefinition = Builders<Channel>.Update.Combine(updates);

            var result = Channels.UpdateOne(filter, updateDefinition);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteChannel(string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var result = Channels.DeleteOne(filter);

            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddCategrory(Category category, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var update = Builders<Channel>.Update.Push(c => c.Categories, category);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteCategrory(string name, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var update = Builders<Channel>.Update.PullFilter(c => c.Categories, c => c.Name == name);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddLiveStream(LiveStream stream, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var update = Builders<Channel>.Update.Push(c => c.Streams, stream);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool UpdateLiveStream(LiveStream stream, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.And(
                Builders<Channel>.Filter.Eq(c => c.Id, channelId),
                Builders<Channel>.Filter.ElemMatch(c => c.Streams, s => s.Id == stream.Id)
            );

            var updates = new List<UpdateDefinition<Channel>>();

            if (stream.Name != null)
                updates.Add(Builders<Channel>.Update.Set("Streams.$.Name", stream.Name));
            if (stream.SavedPath != null)
                updates.Add(Builders<Channel>.Update.Set("Streams.$.SavedPath", stream.SavedPath));
            if (stream.StartDate != DateTime.MinValue)
                updates.Add(Builders<Channel>.Update.Set("Streams.$.StartDate", stream.StartDate));
            if (stream.EndDate != DateTime.MinValue)
                updates.Add(Builders<Channel>.Update.Set("Streams.$.EndDate", stream.EndDate));

            var updateDefinition = Builders<Channel>.Update.Combine(updates);

            var result = Channels.UpdateOne(filter, updateDefinition);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }


    public bool AddPlan(Plan plan, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var update = Builders<Channel>.Update.Push(c => c.Plans, plan);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeletePlan(string planId, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var update = Builders<Channel>.Update.PullFilter(c => c.Plans, p => p.Id == planId);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public Post GetPost(string postId, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.And(
                Builders<Channel>.Filter.Eq(c => c.Id, channelId),
                Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId)
            );

            var projection = Builders<Channel>.Projection.Include(c => c.Posts);

            var channel = Channels.Find(filter).Project<Channel>(projection).FirstOrDefault();

            if (channel == null || channel.Posts == null)
                return null;

            return channel.Posts.FirstOrDefault(p => p.Id == postId);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
    }

    public bool AddPost(Post post, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var update = Builders<Channel>.Update.Push(c => c.Posts, post);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool UpdatePost(Post post, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.And(
                Builders<Channel>.Filter.Eq(c => c.Id, channelId),
                Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == post.Id)
            );

            var updates = new List<UpdateDefinition<Channel>>();

            if (post.Content != null)
                updates.Add(Builders<Channel>.Update.Set("Posts.$.Content", post.Content));
            if (post.Title != null)
                updates.Add(Builders<Channel>.Update.Set("Posts.$.Title", post.Title));
            updates.Add(Builders<Channel>.Update.Set("Posts.$.IsSponsored", post.IsSponsored));

            var updateDefinition = Builders<Channel>.Update.Combine(updates);

            var result = Channels.UpdateOne(filter, updateDefinition);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeletePost(string postId, string channelId)
    {
        try
        {
            var filter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var update = Builders<Channel>.Update.PullFilter(
                c => c.Posts,
                p => p.Id == postId
            );

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddComment(Comment comment, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.Push("Posts.$.Comments", comment);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteComment(string commentId, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.PullFilter("Posts.$.Comments", Builders<Comment>.Filter.Eq(c => c.Id, commentId));

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddRating(Rating rating, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.Push("Posts.$.Ratings", rating);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteRating(string ratingId, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.PullFilter("Posts.$.Ratings", Builders<Rating>.Filter.Eq(c => c.Id, ratingId));

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddImage(Image image, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.Push("Posts.$.Images", image);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteImage(string imageId, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.PullFilter("Posts.$.Images", Builders<Image>.Filter.Eq(c => c.Id, imageId));

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool AddAttachment(Attachment attachment, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.Push("Posts.$.Attachments", attachment);

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteAttachment(string attachmentId, string channelId, string postId)
    {
        try
        {
            var channelFilter = Builders<Channel>.Filter.Eq(c => c.Id, channelId);
            var postFilter = Builders<Channel>.Filter.ElemMatch(c => c.Posts, p => p.Id == postId);
            var filter = Builders<Channel>.Filter.And(channelFilter, postFilter);

            var update = Builders<Channel>.Update.PullFilter("Posts.$.Attachments", Builders<Attachment>.Filter.Eq(c => c.Id, attachmentId));

            var result = Channels.UpdateOne(filter, update);

            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public List<Message> GetMessagesTo(string userId)
    {
        try
        {
            var filter = Builders<Message>.Filter.Eq(m => m.RecipientId, userId);

            var messages = Messages.Find(filter).ToList();

            return messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
    }

    public List<Message> GetMessagesFrom(string userId)
    {
        try
        {
            var filter = Builders<Message>.Filter.Eq(m => m.SenderId, userId);

            var messages = Messages.Find(filter).ToList();

            return messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
    }

    public bool AddMessage(Message message)
    {
        try
        {
            if (string.IsNullOrEmpty(message.SenderId) || string.IsNullOrEmpty(message.RecipientId))
                return false;

            Messages.InsertOne(message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

    public bool DeleteMessage(string messageId)
    {
        try
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, messageId);

            var result = Messages.DeleteOne(filter);

            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return false;
        }
    }

}