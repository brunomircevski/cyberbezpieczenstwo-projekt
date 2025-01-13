using Microsoft.EntityFrameworkCore;
using BDwAS_projekt.Models;

namespace BDwAS_projekt.Data;

public interface IDbContext
{
    public List<User> GetUsers(); 
    public User GetUser(string userId); 
    public bool AddUser(User user);
    public bool UpdateUser(User user);
    public bool DeleteUser(string userId);
    public bool VerifyUser(string userId);

    public bool AddSubscription(Subscription subscription, string userId);
    public bool AddPayment(Payment payment, string subscriptionId, string userId);

    public List<Channel> GetChannels(); //Zwraca tylko kanały bez elementów na listach
    public Channel GetChannel(string channelId); //Zwraca wszystko co zawiera kanał na listach, zwraca posty, ale bez komentarzy, ocen itd
    public bool AddChannel(Channel channel);
    public bool UpdateChannel(Channel channel);
    public bool DeleteChannel(string channelId);

    public bool AddCategrory(Category category, string channelId);
    public bool DeleteCategrory(string name, string channelId);

    public bool AddLiveStream(LiveStream stream, string channelId);
    public bool UpdateLiveStream(LiveStream stream, string channelId);

    public bool AddPlan(Plan plan, string channelId);
    public bool DeletePlan(string planId, string channelId);

    public Post GetPost(string postId, string channelId); //Zwraca wszystko co zawiera post, w tym komentarze, oceny, itd
    public bool AddPost(Post post, string channelId);
    public bool UpdatePost(Post post, string channelId);
    public bool DeletePost(string postId, string channelId);

    public bool AddComment(Comment comment, string channelId, string postId);
    public bool DeleteComment(string commentId, string channelId, string postId);

    public bool AddRating(Rating rating, string channelId, string postId);
    public bool DeleteRating(string ratingId, string channelId, string postId);

    public bool AddImage(Image image, string channelId, string postId);
    public bool DeleteImage(string imageId, string channelId, string postId);

    public bool AddAttachment(Attachment attachment, string channelId, string postId);
    public bool DeleteAttachment(string attachmentId, string channelId, string postId);

    public List<Message> GetMessagesTo(string userId); //Zwraca wszystkie wiadomości wysłane do użytkownika o danym userId
    public List<Message> GetMessagesFrom(string userId); //Zwraca wszystkie wiadomości wysłane przez użytkownika o danym userId
    public bool AddMessage(Message message);
    public bool DeleteMessage(string messageId);
}