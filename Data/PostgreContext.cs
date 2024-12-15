using BDwAS_projekt.Models;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace BDwAS_projekt.Data
{
    public class PostgreContext : DbContext, IDbContext
    {
        private readonly ILogger<PostgreContext> _logger;

        private DbSet<User> Users { get; set; }
        private DbSet<Subscription> Subscriptions { get; set; }
        private DbSet<Rating> Ratings { get; set; }
        private DbSet<Post> Posts { get; set; }
        private DbSet<Plan> Plans { get; set; }
        private DbSet<Payment> Payments { get; set; }
        private DbSet<Message> Messages { get; set; }
        private DbSet<LiveStream> LiveStreams { get; set; }
        private DbSet<Image> Images { get; set; }
        private DbSet<Comment> Comments { get; set; }
        private DbSet<Channel> Channels { get; set; }
        private DbSet<Category> Categories { get; set; }
        private DbSet<Attachment> Attachments { get; set; }

        public PostgreContext(DbContextOptions<PostgreContext> options, ILogger<PostgreContext> logger) : base(options)
        {
            _logger = logger;
        }

        public bool AddAttachment(Attachment attachment, string channelId, string postId)
        {
            try
            {
                var post = Posts.FirstOrDefault(p => p.Id == postId && p.Channel.Id == channelId);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} in channel {ChannelId} not found.", postId, channelId);
                    return false;
                }

                attachment.Post = post;
                Attachments.Add(attachment);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attachment to post {PostId} in channel {ChannelId}.", postId, channelId);
                return false;
            }
        }

        public bool AddCategrory(Category category, string channelId)
        {
            try
            {
                var channel = Channels.Include(c => c.Categories).FirstOrDefault(c => c.Id == channelId);

                if (channel == null)
                {
                    _logger.LogWarning("Channel with ID {ChannelId} not found.", channelId);
                    return false;
                }

                channel.Categories.Add(category);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category to channel {ChannelId}.", channelId);
                return false;
            }
        }

        public bool AddChannel(Channel channel)
        {
            try
            {
                Channels.Add(channel);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding channel.");
                return false;
            }
        }

        public bool AddComment(Comment comment, string channelId, string postId)
        {
            try
            {
                var post = Posts.FirstOrDefault(p => p.Id == postId && p.Channel.Id == channelId);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} in channel {ChannelId} not found.", postId, channelId);
                    return false;
                }

                comment.Post = post;
                Comments.Add(comment);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to post {PostId} in channel {ChannelId}.", postId, channelId);
                return false;
            }
        }

        public bool AddImage(Image image, string channelId, string postId)
        {
            try
            {
                var post = Posts.FirstOrDefault(p => p.Id == postId && p.Channel.Id == channelId);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} in channel {ChannelId} not found.", postId, channelId);
                    return false;
                }

                image.Post = post;
                Images.Add(image);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding image to post {PostId} in channel {ChannelId}.", postId, channelId);
                return false;
            }
        }

        public bool AddLiveStream(LiveStream stream, string channelId)
        {
            try
            {
                var channel = Channels.FirstOrDefault(c => c.Id == channelId);
                if (channel == null)
                {
                    _logger.LogWarning("Channel with ID {ChannelId} not found.", channelId);
                    return false;
                }

                stream.Channel = channel;
                LiveStreams.Add(stream);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding live stream to channel {ChannelId}.", channelId);
                return false;
            }
        }

        public bool AddMessage(Message message)
        {
            try
            {
                Messages.Add(message);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding message.");
                return false;
            }
        }

        public bool AddPayment(Payment payment, string subscriptionId, string userId)
        {
            try
            {
                var user = Users.Include(u => u.Subscriptions).ThenInclude(s => s.Payments)
                                .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return false;
                }

                var subscription = user.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                if (subscription == null)
                {
                    _logger.LogWarning("Subscription with ID {SubscriptionId} for user {UserId} not found.", subscriptionId, userId);
                    return false;
                }

                subscription.Payments.Add(payment);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding payment for subscription {SubscriptionId} and user {UserId}.", subscriptionId, userId);
                return false;
            }
        }

        public bool AddPlan(Plan plan, string channelId)
        {
            try
            {
                var channel = Channels.FirstOrDefault(c => c.Id == channelId);
                if (channel == null)
                {
                    _logger.LogWarning("Channel with ID {ChannelId} not found.", channelId);
                    return false;
                }

                plan.Channel = channel;
                Plans.Add(plan);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plan to channel {ChannelId}.", channelId);
                return false;
            }
        }

        public bool AddPost(Post post, string channelId)
        {
            try
            {
                var channel = Channels.FirstOrDefault(c => c.Id == channelId);
                if (channel == null)
                {
                    _logger.LogWarning("Channel with ID {ChannelId} not found.", channelId);
                    return false;
                }

                post.Channel = channel;
                Posts.Add(post);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding post to channel {ChannelId}.", channelId);
                return false;
            }
        }

        public bool AddRating(Rating rating, string channelId, string postId)
        {
            try
            {
                var post = Posts.FirstOrDefault(p => p.Id == postId && p.Channel.Id == channelId);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} in channel {ChannelId} not found.", postId, channelId);
                    return false;
                }

                rating.Post = post;
                Ratings.Add(rating);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding rating to post {PostId} in channel {ChannelId}.", postId, channelId);
                return false;
            }
        }

        public bool AddSubscription(Subscription subscription, string userId)
        {
            try
            {
                var user = Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return false;
                }

                subscription.User = user;
                Subscriptions.Add(subscription);

                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding subscription for user {UserId}.", userId);
                return false;
            }
        }

        public bool AddUser(User user)
        {
            try
            {
                Users.Add(user);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user.");
                return false;
            }
        }

        public bool DeleteAttachment(string attachmentId, string channelId, string postId)
        {
            try
            {
                var attachment = Attachments
                    .FirstOrDefault(a => a.Id == attachmentId && a.Post.Id == postId && a.Post.Channel.Id == channelId);

                if (attachment == null)
                {
                    _logger.LogWarning("Attachment with ID {AttachmentId} not found in post {PostId} and channel {ChannelId}.", attachmentId, postId, channelId);
                    return false;
                }

                Attachments.Remove(attachment);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attachment with ID {AttachmentId}.", attachmentId);
                return false;
            }
        }

        public bool DeleteCategrory(string name, string channelId)
        {
            try
            {
                var category = Categories.FirstOrDefault(c => c.Name == name);

                if (category == null)
                {
                    _logger.LogWarning("Category with name {Name} not found.", name);
                    return false;
                }

                var channelsWithCategory = Channels
                    .Where(c => c.Categories.Any(cat => cat.Id == category.Id))
                    .ToList();

                foreach (var channel in channelsWithCategory)
                {
                    channel.Categories.Remove(category);
                }

                Categories.Remove(category);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {Name}.", name);
                return false;
            }
        }


        public bool DeleteChannel(string channelId)
        {
            try
            {
                var channel = Channels.FirstOrDefault(c => c.Id == channelId);

                if (channel == null)
                {
                    _logger.LogWarning("Channel with ID {ChannelId} not found.", channelId);
                    return false;
                }

                Channels.Remove(channel);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting channel with ID {ChannelId}.", channelId);
                return false;
            }
        }

        public bool DeleteComment(string commentId, string channelId, string postId)
        {
            try
            {
                var comment = Comments
                    .FirstOrDefault(c => c.Id == commentId && c.Post.Id == postId && c.Post.Channel.Id == channelId);

                if (comment == null)
                {
                    _logger.LogWarning("Comment with ID {CommentId} not found in post {PostId} and channel {ChannelId}.", commentId, postId, channelId);
                    return false;
                }

                Comments.Remove(comment);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID {CommentId}.", commentId);
                return false;
            }
        }

        public bool DeleteImage(string imageId, string channelId, string postId)
        {
            try
            {
                var image = Images
                    .FirstOrDefault(i => i.Id == imageId && i.Post.Id == postId && i.Post.Channel.Id == channelId);

                if (image == null)
                {
                    _logger.LogWarning("Image with ID {ImageId} not found in post {PostId} and channel {ChannelId}.", imageId, postId, channelId);
                    return false;
                }

                Images.Remove(image);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image with ID {ImageId}.", imageId);
                return false;
            }
        }

        public bool DeleteMessage(string messageId)
        {
            try
            {
                var message = Messages.FirstOrDefault(m => m.Id == messageId);

                if (message == null)
                {
                    _logger.LogWarning("Message with ID {MessageId} not found.", messageId);
                    return false;
                }

                Messages.Remove(message);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message with ID {MessageId}.", messageId);
                return false;
            }
        }

        public bool DeletePlan(string planId, string channelId)
        {
            try
            {
                var plan = Plans
                    .FirstOrDefault(p => p.Id == planId && p.Channel.Id == channelId);

                if (plan == null)
                {
                    _logger.LogWarning("Plan with ID {PlanId} not found in channel {ChannelId}.", planId, channelId);
                    return false;
                }

                Plans.Remove(plan);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plan with ID {PlanId} in channel {ChannelId}.", planId, channelId);
                return false;
            }
        }

        public bool DeletePost(string postId, string channelId)
        {
            try
            {
                var post = Posts
                    .FirstOrDefault(p => p.Id == postId && p.Channel.Id == channelId);

                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found in channel {ChannelId}.", postId, channelId);
                    return false;
                }

                Posts.Remove(post);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post with ID {PostId} in channel {ChannelId}.", postId, channelId);
                return false;
            }
        }

        public bool DeleteRating(string ratingId, string channelId, string postId)
        {
            try
            {
                var rating = Ratings
                    .FirstOrDefault(r => r.Id == ratingId && r.Post.Id == postId && r.Post.Channel.Id == channelId);

                if (rating == null)
                {
                    _logger.LogWarning("Rating with ID {RatingId} not found in post {PostId} and channel {ChannelId}.", ratingId, postId, channelId);
                    return false;
                }

                Ratings.Remove(rating);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rating with ID {RatingId}.", ratingId);
                return false;
            }
        }

        public bool DeleteUser(string userId)
        {
            try
            {
                var user = Users.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return false;
                }

                Users.Remove(user);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}.", userId);
                return false;
            }
        }


        public Channel GetChannel(string channelId)
        {
            try
            {
                return Channels.FirstOrDefault(c => c.Id == channelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving channel with ID {ChannelId}.", channelId);
                return null;
            }
        }

        public List<Channel> GetChannels()
        {
            try
            {
                return Channels.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving channels.");
                return new List<Channel>();
            }
        }

        public List<Message> GetMessagesFrom(string userId)
        {
            try
            {
                return Messages.Where(m => m.Sender.Id == userId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages from user with ID {UserId}.", userId);
                return new List<Message>();
            }
        }

        public List<Message> GetMessagesTo(string userId)
        {
            try
            {
                return Messages.Where(m => m.Recipient.Id == userId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages to user with ID {UserId}.", userId);
                return new List<Message>();
            }
        }

        public Post GetPost(string postId, string channelId)
        {
            try
            {
                return Posts.FirstOrDefault(p => p.Id == postId && p.Channel.Id == channelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving post with ID {PostId} in channel {ChannelId}.", postId, channelId);
                return null;
            }
        }

        public User GetUser(string userId)
        {
            try
            {
                return Users.FirstOrDefault(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}.", userId);
                return null;
            }
        }

        public List<User> GetUsers()
        {
            try
            {
                return Users.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users.");
                return new List<User>();
            }
        }


        public bool UpdateChannel(Channel channel)
        {
            try
            {
                var existingChannel = Channels.FirstOrDefault(c => c.Id == channel.Id);
                if (existingChannel == null)
                {
                    _logger.LogWarning("Channel with ID {ChannelId} not found.", channel.Id);
                    return false;
                }

                Entry(existingChannel).CurrentValues.SetValues(channel);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating channel with ID {ChannelId}.", channel.Id);
                return false;
            }
        }

        public bool UpdateLiveStream(LiveStream stream, string channelId)
        {
            try
            {
                var existingStream = LiveStreams.FirstOrDefault(ls => ls.Id == stream.Id && ls.Channel.Id == channelId);
                if (existingStream == null)
                {
                    _logger.LogWarning("LiveStream with ID {StreamId} not found in channel {ChannelId}.", stream.Id, channelId);
                    return false;
                }

                Entry(existingStream).CurrentValues.SetValues(stream);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LiveStream with ID {StreamId} in channel {ChannelId}.", stream.Id, channelId);
                return false;
            }
        }

        public bool UpdatePost(Post post, string channelId)
        {
            try
            {
                var existingPost = Posts.FirstOrDefault(p => p.Id == post.Id && p.Channel.Id == channelId);
                if (existingPost == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found in channel {ChannelId}.", post.Id, channelId);
                    return false;
                }

                Entry(existingPost).CurrentValues.SetValues(post);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post with ID {PostId} in channel {ChannelId}.", post.Id, channelId);
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                var existingUser = Users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", user.Id);
                    return false;
                }

                Entry(existingUser).CurrentValues.SetValues(user);
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}.", user.Id);
                return false;
            }
        }

        public bool VerifyUser(string userId)
        {
            try
            {
                var existingUser = Users.FirstOrDefault(u => u.Id == userId);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return false;
                }

                existingUser.IsVerified = true;
                return SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying user with ID {UserId}.", userId);
                return false;
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SendMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Channel>()
                .HasOne(c => c.Owner)
                .WithOne(u => u.Channel)
                .HasForeignKey<Channel>(c => c.OwnerId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.AuthorId);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Channel)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.ChannelId);

            modelBuilder.Entity<Payment>(entity =>
            {
                // Konfiguracja dla JSON
                entity.Property(p => p.Details)
                    .HasColumnType("jsonb") // PostgreSQL JSONB
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = false }),
                        v => JsonSerializer.Deserialize<PaymentDetails>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

                entity.HasOne(p => p.Subscription)
                    .WithMany(s => s.Payments)
                    .HasForeignKey(p => p.SubscriptionId);
            });


        }
    }
}
