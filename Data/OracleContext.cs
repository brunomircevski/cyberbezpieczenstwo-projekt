using BDwAS_projekt.Models;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;
namespace BDwAS_projekt.Data;

public class OracleContext : IDbContext
{
    private readonly string _connectionString;
    public OracleContext(string connectionString)
    {
        _connectionString = connectionString;
    }


    private bool isInDataBase(string tableName, string tableIdName, string Id){
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string checkQuery = @$"
                    SELECT COUNT(*) 
                    FROM {tableName} 
                    WHERE {tableIdName} = '{Id}'";

                using (var command = new OracleCommand(checkQuery, connection))
                {

                    connection.Open();
                    var result = Convert.ToInt32(command.ExecuteScalar());
                    if (result == 0)
                        return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking. \nException Message: {ex.Message}");
        }
    }

    public bool AddAttachment(Attachment attachment, string channelId, string postId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");

                if(!isInDataBase("Posts", "PostId", postId))
                    throw new Exception($"Post {postId} not found.");

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertAttachmentQuery = @$"
                            INSERT INTO Attachments VALUES (
                                Attachment('{attachment.Id}', '{attachment.Name}', '{attachment.Path}', {attachment.Size})
                        )";

                        using (var insertAttachmentCommand = new OracleCommand(insertAttachmentQuery, connection))
                        {
                            int rowsAffected = insertAttachmentCommand.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add attachment to Attachments.");

                            Console.WriteLine("Attachment added successfully to Attachments.");
                        }

                        string updatePostQuery = @$"
                            UPDATE Posts p
                            SET p.Attachments = p.Attachments MULTISET UNION 
                                AttachmentList((SELECT REF(a) FROM Attachments a WHERE a.AttachmentId = '{attachment.Id}'))
                            WHERE p.PostId = '{postId}'";

                        using (var updatePostCommand = new OracleCommand(updatePostQuery, connection))
                        {
                            int rowsAffected = updatePostCommand.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add attachment to Post.");
                            Console.WriteLine("Attachment added successfully to Post.");
                        }

                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding attachment to post {postId} in channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }




    public bool AddCategrory(Category category, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertCategoryQuery = @$"
                            INSERT INTO ChannelCategories VALUES (
                                ChannelCategory('{category.Id}', '{category.Name}', {category.MinimumAge})
                            )";

                        using (var insertCategoryCommand = new OracleCommand(insertCategoryQuery, connection))
                        {

                            int rowsAffected = insertCategoryCommand.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add category to ChannelCategories.");

                            Console.WriteLine("Category added successfully to ChannelCategories.");
                        }

                        string updateChannelCategoriesQuery = @$"
                            UPDATE Channels c
                            SET c.Categories = 
                                c.Categories MULTISET UNION 
                                ChannelCategoriesList((SELECT REF(cc) FROM ChannelCategories cc WHERE cc.ChannelCategoryId = '{category.Id}'))
                            WHERE c.ChannelId = '{channelId}'";

                        using (var updateChannelCommand = new OracleCommand(updateChannelCategoriesQuery, connection))
                        {

                            int rowsAffected = updateChannelCommand.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                                throw new Exception("Failed to update categories in the Channel.");

                            Console.WriteLine("Category added successfully to Channel.");
                        }

                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding category to channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddChannel(Channel channel)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                if(!isInDataBase("Users","UserId",channel.OwnerId))
                    throw new Exception($"User {channel.OwnerId} now found");

                string creationDateString = channel.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");
                string insertChannelQuery = @$"
                    INSERT INTO Channels VALUES (
                        Channel(
                            '{channel.Id}', 
                            '{channel.Name}',
                            '{channel.Description}',
                            TO_DATE('{creationDateString}', 'YYYY-MM-DD HH24:MI:SS'),
                            (SELECT REF(u) FROM Users u WHERE u.UserId = '{channel.OwnerId}'),
                            ChannelCategoriesList(),
                            PlansList(),
                            SubscriptionList(),
                            StreamList()
                        )
                    )";

                                    
                using (var command = new OracleCommand(insertChannelQuery, connection))
                {

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new Exception("Failed to add channel.");
            
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding channel. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddComment(Comment comment, string channelId, string postId)
    {
        try
        {
            if(!isInDataBase("Users", "UserId", comment.AuthorId))
                    throw new Exception($"User with ID {comment.AuthorId} not found.");

            if(!isInDataBase("Channels", "ChannelId", channelId))
                throw new Exception($"Channel with ID {channelId} not found.");

            if(!isInDataBase("Posts", "PostId", postId))
                throw new Exception($"Post with ID {postId} in channel {channelId} not found.");

            if(isInDataBase("Comments", "CommentId", comment.Id))
                throw new Exception($"Comment with ID {comment.Id} already exist.");


            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        string creationDateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string insertCommentQuery = @$"
                            INSERT INTO Comments VALUES (
                            CommentType('{comment.Id}', '{comment.Content}', TO_DATE('{creationDateString}', 'YYYY-MM-DD HH24:MI:SS'), 
                            (SELECT REF(u) FROM Users u WHERE u.UserId = '{comment.AuthorId}'))
                        )";
                        Console.WriteLine(insertCommentQuery);

                        using (var command = new OracleCommand(insertCommentQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add comment.");
                            string updatePostQuery = @$"
                                UPDATE Posts p
                                SET p.Comments = p.Comments MULTISET UNION 
                                    CommentsList(
                                        (SELECT REF(c) FROM Comments c WHERE c.CommentId = '{comment.Id}')
                                    )
                                WHERE p.PostId = '{postId}'";

                            using (var updateCommand = new OracleCommand(updatePostQuery, connection))
                            {

                                int updateRowsAffected = updateCommand.ExecuteNonQuery();
                                if (updateRowsAffected <= 0)
                                    throw new Exception("Failed to update post with new comment.");

                                Console.WriteLine("Comment added successfully.");
                            }
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding comment to post {postId} in channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddImage(Image image, string channelId, string postId)
    {
        try
        {
            if(!isInDataBase("Channels", "ChannelId", channelId))
                throw new Exception($"Channel {channelId} not found.");

            if(!isInDataBase("Posts", "PostId", postId))
                throw new Exception($"Post with ID {postId} in channel {channelId} not found.");

            if(isInDataBase("Images", "ImageId", image.Id))
                throw new Exception($"Image with ID {image.Id} already exist.");


            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        

                        string insertImageQuery = @$"
                            INSERT INTO Images VALUES (
                                Image('{image.Id}', '{image.Path}')
                        )";

                        using (var command = new OracleCommand(insertImageQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add image.");

                            string updatePostQuery = @$"
                                UPDATE Posts p
                                SET p.Images = p.Images MULTISET UNION 
                                    ImagesList(
                                        (SELECT REF(i) FROM Images i WHERE i.ImageId = '{image.Id}')
                                    )
                                WHERE p.PostId = '{postId}'";

                            using (var updateCommand = new OracleCommand(updatePostQuery, connection))
                            {

                                int updateRowsAffected = updateCommand.ExecuteNonQuery();
                                if (updateRowsAffected <= 0)
                                    throw new Exception("Failed to update post with new image.");

                                Console.WriteLine("Image added successfully.");
                            }
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
               
            }
            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding image to post {postId} in channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddLiveStream(LiveStream stream, string channelId)
    {
        try
        {
            if(!isInDataBase("Channels", "ChannelId", channelId))
                throw new Exception($"Channel {channelId} not found.");

            if(isInDataBase("Streams", "StreamId", stream.Id))
                throw new Exception($"Stream with ID {stream.Id} already exist.");
            

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string startOfDateString = stream.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                        string insertStreamQuery = @$"
                            INSERT INTO Streams VALUES (
                                StreamType('{stream.Id}', '{stream.Name}', '{stream.SavedPath}',
                                TO_DATE('{startOfDateString}', 'YYYY-MM-DD HH24:MI:SS')";

                        if (stream.EndDate != null)
                        {
                            string isEndedString = stream.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
                            insertStreamQuery += @$",
                                TO_DATE('{isEndedString}', 'YYYY-MM-DD HH24:MI:SS')";
                        }
                            
                        insertStreamQuery += @")
                        )";

                        using (var command = new OracleCommand(insertStreamQuery, connection))
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add stream.");

                            string updateChannelQuery = @$"
                                UPDATE Channels c
                                SET c.Streams = c.Streams MULTISET UNION 
                                    StreamList(
                                        (SELECT REF(s) FROM Streams s WHERE s.StreamId = '{stream.Id}')
                                    )
                                WHERE c.ChannelId = '{channelId}'";

                            using (var updateCommand = new OracleCommand(updateChannelQuery, connection))
                            {

                                int updateRowsAffected = updateCommand.ExecuteNonQuery();
                                if (updateRowsAffected <= 0)
                                    throw new Exception("Failed to update channel with new stream.");

                                Console.WriteLine("Comment added successfully.");
                            }
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
               
            }
            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding live stream to channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddMessage(Message message)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(!isInDataBase("Users","UserId",message.SenderId))
                    throw new Exception($"Sender with id {message.SenderId} not found.");
                
                if(!isInDataBase("Users","UserId",message.RecipientId))
                    throw new Exception($"Recipient with id {message.RecipientId} not found.");

                if(isInDataBase("Messages","MessageId",message.Id))
                    throw new Exception($"Message with id {message.Id} already exist.");

                string dateTimeOfSentString = message.Date.ToString("yyyy-MM-dd HH:mm:ss");
                string insertMessageQuery = @$"
                    INSERT INTO Messages VALUES (
                        Message(
                            '{message.Id}',
                            TO_DATE('{dateTimeOfSentString}', 'YYYY-MM-DD HH24:MI:SS'),
                            '{message.Content}',
                            (SELECT REF(u) FROM Users u WHERE u.UserId = '{message.SenderId}'),
                            (SELECT REF(u) FROM Users u WHERE u.UserId = '{message.RecipientId}')
                        )
                    )";

                connection.Open();                   
                using (var command = new OracleCommand(insertMessageQuery, connection))
                {

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new Exception("Failed to add message.");
            
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding message. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddPayment(Payment payment, string subscriptionId, string userId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(!isInDataBase("Users","UserId",userId))
                    throw new Exception($"User with id {userId} not found.");

                if(!isInDataBase("Subscriptions","SubscriptionId",subscriptionId))
                    throw new Exception($"Subscription with id {subscriptionId} not found.");
                
                if(isInDataBase("Payments","PaymentId",payment.Id))
                    throw new Exception($"Payment with id {payment.Id} already exist.");

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string paymentDateString = payment.Details.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss");
                        string insertPaymentQuery = @$"
                            INSERT INTO Payments VALUES (
                                Payment(
                                    '{payment.Id}',
                                    (SELECT REF(u) FROM Users u WHERE u.UserId = '{userId}'),
                                    PaymentDetailsType(
                                        TO_DATE('{paymentDateString}', 'YYYY-MM-DD HH24:MI:SS'),
                                        {payment.Details.AddedDays}, 
                                        {payment.Details.FullPrice.ToString("F2", CultureInfo.InvariantCulture)},
                                        {payment.Details.PaidPrice.ToString("F2", CultureInfo.InvariantCulture)},
                                         {payment.Details.Discount.ToString("F2", CultureInfo.InvariantCulture)}
                                    )
                                )
                            )";

                        using (var command = new OracleCommand(insertPaymentQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add payment.");
                    
                        }

                        string updateSubscriptionQuery = @$"
                            UPDATE Subscriptions s
                            SET s.Payments = s.Payments MULTISET UNION 
                                PaymentsList(
                                    (SELECT REF(p) FROM Payments p WHERE p.PaymentId = '{payment.Id}')
                                )
                            WHERE s.SubscriptionId = '{subscriptionId}'";

                        using (var updateCommand = new OracleCommand(updateSubscriptionQuery, connection))
                        {

                            int updateRowsAffected = updateCommand.ExecuteNonQuery();
                            if (updateRowsAffected <= 0)
                                throw new Exception("Failed to update subscription with new payment.");

                            Console.WriteLine("Payment added successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding payment. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddPlan(Plan plan, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");

                if(isInDataBase("Plans", "PlanId", plan.Id))
                    throw new Exception($"Plan with id {plan.Id} already exist.");
                

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertPlanQuery = @$"
                            INSERT INTO Plans VALUES (
                                PlanType('{plan.Id}', '{plan.Name}', {plan.Days}, 
                                {plan.Price.ToString("F2", CultureInfo.InvariantCulture)}, 
                                {plan.Discount.ToString("F2", CultureInfo.InvariantCulture)})
                            )";
                        using (var command = new OracleCommand(insertPlanQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add plan.");
                    
                        }

                        string updateChannelQuery = @$"
                            UPDATE Channels c
                            SET c.Plans = c.Plans MULTISET UNION 
                                PlansList(
                                    (SELECT REF(p) FROM Plans p WHERE p.PlanId = '{plan.Id}')
                                )
                            WHERE c.ChannelId = '{channelId}'";

                        using (var updateCommand = new OracleCommand(updateChannelQuery, connection))
                        {

                            int updateRowsAffected = updateCommand.ExecuteNonQuery();
                            if (updateRowsAffected <= 0)
                                throw new Exception("Failed to update channel with new plan.");

                            Console.WriteLine("Plan added successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding plan to channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddPost(Post post, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");

                if(isInDataBase("Posts", "PostId", post.Id))
                    throw new Exception($"Post with id {post.Id} already exist.");
                

                connection.Open();
                string isSponsoredString = post.IsSponsored ? "1" : "0";
                string creationDateString = post.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");                   
                string insertPostQuery = @$"
                    INSERT INTO Posts VALUES (
                        Post(
                            '{post.Id}', 
                            '{post.Title}',
                            '{post.Content}', 
                            TO_DATE('{creationDateString}', 'YYYY-MM-DD HH24:MI:SS'), 
                            '{isSponsoredString}',
                            (SELECT REF(c) FROM Channels c WHERE c.ChannelId = '{channelId}'),
                            CommentsList(),
                            RatingsList(),
                            ImagesList(),
                            AttachmentList()
                        )
                    )";

                using (var command = new OracleCommand(insertPostQuery, connection))
                {

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new Exception("Failed to add post.");
            
                }                
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding post. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddRating(Rating rating, string channelId, string postId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(!isInDataBase("Users", "UserId", rating.AuthorId))
                    throw new Exception($"User {rating.AuthorId} not found.");

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("Posts", "PostId", postId))
                    throw new Exception($"Post {postId} not found.");

                if(isInDataBase("Ratings", "RatingId", rating.Id))
                    throw new Exception($"Rating with id {rating.Id} already exist.");
                

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertRatingQuery = @$"
                            INSERT INTO Ratings VALUES (
                                Rating('{rating.Id}', {rating.Value},
                                    (SELECT REF(u) FROM Users u WHERE u.UserId = '{rating.AuthorId}')
                                )
                            )";

                        using (var command = new OracleCommand(insertRatingQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to add rating.");

                            Console.WriteLine("Rating added successfully.");
                    
                        }

                        string updatePostQuery = @$"
                            UPDATE Posts p
                            SET p.Ratings = p.Ratings MULTISET UNION 
                                RatingsList(
                                    (SELECT REF(r) FROM Ratings r WHERE r.RatingId = '{rating.Id}')
                                )
                            WHERE p.PostId = '{postId}'";

                        using (var updateCommand = new OracleCommand(updatePostQuery, connection))
                        {

                            int updateRowsAffected = updateCommand.ExecuteNonQuery();
                            if (updateRowsAffected <= 0)
                                throw new Exception("Failed to update post with new rating.");

                            Console.WriteLine("Rating added to post successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding rating to post {postId} in channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }


    public bool AddSubscription(Subscription subscription, string userId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(!isInDataBase("Users", "UserId", userId))
                    throw new Exception($"User {userId} not found.");
                
                if(isInDataBase("Subscriptions", "SubscriptionId", subscription.Id))
                    throw new Exception($"Subscription {subscription.Id} already exist.");

                connection.Open();
                string startDateString = subscription.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                string endDateString = subscription.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
                string autoRenewString = subscription.AutoRenew ? "1" : "0";                   
                string insertSubscriptionQuery = @$"
                    INSERT INTO Subscriptions VALUES (
                        SubscriptionType(
                            '{subscription.Id}', 
                            (SELECT REF(u) FROM Users u WHERE u.UserId = '{userId}'),
                            TO_DATE('{startDateString}', 'YYYY-MM-DD HH24:MI:SS'), 
                            TO_DATE('{endDateString}', 'YYYY-MM-DD HH24:MI:SS'), 
                            '{autoRenewString}',
                            PaymentsList()
                        )
                    )";

                using (var command = new OracleCommand(insertSubscriptionQuery, connection))
                {

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new Exception("Failed to add subscription.");
            
                }                
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding subscription for user {userId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool AddUser(User user)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(isInDataBase("Users", "UserId", user.Id))
                    throw new Exception($"User {user.Id} already exist.");

                connection.Open();
                string isVerifiedString = user.IsVerified ? "1" : "0";
                string dateOfRegisterString = user.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss");                 
                string insertSubscriptionQuery = @$"
                    INSERT INTO Users VALUES (
                        UserType(
                            '{user.Id}', '{isVerifiedString}' ,
                            TO_DATE('{dateOfRegisterString}', 'YYYY-MM-DD HH24:MI:SS'),
                            UserProfile('{user.FirstName}' ,'{ user.LastName}' ,'{user.Email}')
                        )
                    )";

                using (var command = new OracleCommand(insertSubscriptionQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new Exception("Failed to add user.");
            
                }                
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding user. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool DeleteAttachment(string attachmentId, string channelId, string postId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("Posts", "PostId", postId))
                    throw new Exception($"Post {postId} not found.");

                if(!isInDataBase("Attachments", "AttachmentId", attachmentId))
                    throw new Exception($"Attachment {attachmentId} not found.");
                

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteAttachmentFromPostRatingQuery = @$"UPDATE Posts p
                            SET p.Attachments = 
                                CAST(
                                    MULTISET(
                                        SELECT al.COLUMN_VALUE
                                        FROM TABLE(p.Attachments) al
                                        WHERE al.COLUMN_VALUE != (SELECT REF(a) FROM Attachments a WHERE a.AttachmentId = '{attachmentId}')
                                    ) AS AttachmentList
                                )
                            WHERE p.PostId = '{postId}'";

                        using (var command = new OracleCommand(deleteAttachmentFromPostRatingQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to delete attachment.");

                            Console.WriteLine("Attachment deleted successfully.");
                    
                        }

                        string deleteAttachmentQuery = @$"
                            DELETE FROM Attachments a
                            WHERE a.AttachmentId = '{attachmentId}'";

                        using (var deleteCommand = new OracleCommand(deleteAttachmentQuery, connection))
                        {

                            int deleteRowsAffected = deleteCommand.ExecuteNonQuery();
                            if (deleteRowsAffected <= 0)
                                throw new Exception("Failed to delete attachment.");

                            Console.WriteLine("Attachment deleted successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting attachment with ID {attachmentId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool DeleteCategrory(string name, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("ChannelCategories", "ChannelCategoryName", name))
                    throw new Exception($"Category {name} not found.");

                

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteCategoryFromAllChannelsQuery = @$"DELETE FROM TABLE (
                            SELECT c.Categories
                            FROM Channels c
                        ) cat
                        WHERE cat.COLUMN_VALUE = (
                            SELECT REF(catRef)
                            FROM ChannelCategories catRef
                            WHERE catRef.ChannelCategoryName = '{name}'
                        )";

                        using (var command = new OracleCommand(deleteCategoryFromAllChannelsQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to remove category ref.");

                            Console.WriteLine("Category ref removed successfully.");
                    
                        }

                        string deleteCategoryQuery = @$"
                            DELETE FROM ChannelCategories c
                            WHERE c.ChannelCategoryName = '{name}'";

                        using (var deleteCommand = new OracleCommand(deleteCategoryQuery, connection))
                        {

                            int deleteRowsAffected = deleteCommand.ExecuteNonQuery();
                            if (deleteRowsAffected <= 0)
                                throw new Exception("Failed to delete category.");

                            Console.WriteLine("Category deleted successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting category {name}. \nException Message: {ex.Message}");
            return false;
        }
    }

    private void DeleteFromList(OracleConnection connection, OracleTransaction transaction, string  mainTableName
    , string innerTableName, string mainTableIdName ,string innerTableIdName,
    string listName, string Id)
    {
        try
        {

            string deleteQuery = @$"
                DELETE FROM {innerTableName}
                WHERE {innerTableIdName} IN (
                    SELECT DEREF(mlist.COLUMN_VALUE).{innerTableIdName}
                    FROM {mainTableName} m, 
                        TABLE(m.{listName}) mlist
                    WHERE m.{mainTableIdName} = '{Id}'
                    )";
            Console.WriteLine($"list {deleteQuery}");
            Console.WriteLine($"list {innerTableName}");
            using (var command = new OracleCommand(deleteQuery, connection))
            {
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
            Console.WriteLine($"list cc{innerTableName}");

        }
        catch (Exception ex)
        {
            throw new Exception($"Error removing from {innerTableName} objects from {mainTableName}: {ex.Message}");
        }
    }


    private List<string> GetIdsByuser(string userId, string tableName, string tableIdName, 
    string tableUserRefName)
    {
        try
        {
            List<string> Ids = new List<string>();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"
                    SELECT t.{tableIdName}
                    FROM {tableName} t
                    WHERE DEREF(t.{tableUserRefName}).UserId = '{userId}'";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Ids.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return Ids;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting Ids for {tableName}: {ex.Message}");
        }
    }


    private void innerDeleteChannel(OracleConnection connection, OracleTransaction transaction, string channelId)
    {
        Console.WriteLine("X");

        List<string> postIds = new List<string>();

        string query = @$"
            SELECT p.PostId
            FROM Posts p
            WHERE DEREF(p.ChannelRef).ChannelId = '{channelId}'";

        using (OracleCommand command = new OracleCommand(query, connection))
        {
            command.Transaction = transaction;
            using (OracleDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    postIds.Add(reader.GetString(0));
                }
            }
        }

        foreach(string postId in postIds)
        {
            innerDeletePost(connection, transaction, postId, channelId);
        }


        DeleteFromList(connection, transaction, "Channels", "Plans", "ChannelId", "PlanId",
        "Plans", channelId);

        DeleteFromList(connection, transaction, "Channels", "Subscriptions", "ChannelId", "SubscriptionId",
        "Subscriptions", channelId);

        DeleteFromList(connection, transaction, "Channels", "Streams", "ChannelId", "StreamId",
        "Streams", channelId);

        string deleteChannelQuery = @$"
            DELETE FROM Channels
            WHERE ChannelId = '{channelId}'";
        using (var command = new OracleCommand(deleteChannelQuery, connection))
        {
            command.Transaction = transaction;
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected <= 0)
                throw new Exception("Failed to remove channel.");
        }
        Console.WriteLine(deleteChannelQuery);
    }



    public bool DeleteChannel(string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if (!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        innerDeleteChannel(connection, transaction, channelId);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error removing channel {channelId}: {ex.Message}");
                        return false;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing channel {channelId}: {ex.Message}");
            return false;
        }
    }

    public bool DeleteComment(string commentId, string channelId, string postId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("Posts", "PostId", postId))
                    throw new Exception($"Post {postId} not found.");
                
                if(!isInDataBase("Comments", "CommentId", commentId))
                    throw new Exception($"Comment {commentId} not found.");

                connection.Open();                   
                string deleteCommentQuery = @$"
                    DELETE FROM Comments c
                    WHERE c.CommentId = '{commentId}'";


                using (var command = new OracleCommand(deleteCommentQuery, connection))
                {

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new Exception("Failed to remove comment.");
            
                }                
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error remove comment {commentId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool DeleteImage(string imageId, string channelId, string postId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("Posts", "PostId", postId))
                    throw new Exception($"Post {postId} not found.");

                if(!isInDataBase("Images", "ImageId", imageId))
                    throw new Exception($"Image {imageId} not found.");
                

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteImageFromPostImagesQuery = @$"UPDATE Posts p
                            SET p.Images = 
                                CAST(
                                    MULTISET(
                                        SELECT il.COLUMN_VALUE
                                        FROM TABLE(p.Images) il
                                        WHERE il.COLUMN_VALUE != (SELECT REF(i) FROM Images i WHERE i.ImageId = '{imageId}')
                                    ) AS ImagesList
                                )
                            WHERE p.PostId = '{postId}'";

                        using (var command = new OracleCommand(deleteImageFromPostImagesQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to delete image.");

                            Console.WriteLine("Image deleted from post successfully.");
                    
                        }

                        string deleteImageQuery = @$"
                            DELETE FROM Images i
                            WHERE i.ImageId = '{imageId}'";

                        using (var deleteCommand = new OracleCommand(deleteImageQuery, connection))
                        {

                            int deleteRowsAffected = deleteCommand.ExecuteNonQuery();
                            if (deleteRowsAffected <= 0)
                                throw new Exception("Failed to delete image.");

                            Console.WriteLine("Image deleted successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting image with ID {imageId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool DeleteMessage(string messageId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Messages", "MessageId", messageId))
                    throw new Exception($"Message with ID {messageId} not found.");

                connection.Open();                   
                string deleteMessageQuery = @$"
                    DELETE FROM Messages m
                    WHERE m.MessageId = '{messageId}'";


                using (var command = new OracleCommand(deleteMessageQuery, connection))
                {

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                        throw new Exception("Failed to remove message.");
            
                }                
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error remove message {messageId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool DeletePlan(string planId, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("Plans", "PlanId", planId))
                    throw new Exception($"Plan {planId} not found.");

                

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deletePlanFromAllChannelsQuery = @$"DELETE FROM TABLE (
                            SELECT c.Plans
                            FROM Channels c
                        ) p
                        WHERE p.COLUMN_VALUE = (
                            SELECT REF(pRef)
                            FROM Plans pRef
                            WHERE pRef.PlanId = '{planId}'
                        )";
                        using (var command = new OracleCommand(deletePlanFromAllChannelsQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to remove plan ref.");

                            Console.WriteLine("Plan ref removed successfully.");
                    
                        }

                        string deletePlanQuery = @$"
                            DELETE FROM Plans p
                            WHERE p.PlanId = '{planId}'";

                        using (var deleteCommand = new OracleCommand(deletePlanQuery, connection))
                        {

                            int deleteRowsAffected = deleteCommand.ExecuteNonQuery();
                            if (deleteRowsAffected <= 0)
                                throw new Exception("Failed to delete plan.");

                            Console.WriteLine("Plan deleted successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting plan with ID {planId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    private void innerDeletePost(OracleConnection connection, OracleTransaction transaction, string postId, string channelId)
    {
        Console.WriteLine("TT");

        DeleteFromList(connection, transaction, "Posts", "Comments", "PostId", "CommentId",
        "Comments", postId);
        Console.WriteLine("TT1");

        DeleteFromList(connection, transaction, "Posts", "Ratings", "PostId", "RatingId",
        "Ratings", postId);

        Console.WriteLine("TT2");
        DeleteFromList(connection, transaction, "Posts", "Images", "PostId", "ImageId",
        "Images", postId);

        Console.WriteLine("TT3");
        DeleteFromList(connection, transaction, "Posts", "Attachments", "PostId", "AttachmentId",
        "Attachments", postId);

        Console.WriteLine("TTT");

        string deletePostQuery = @$"
            DELETE FROM Posts p
            WHERE p.PostId = '{postId}'";

        using (var deleteCommand = new OracleCommand(deletePostQuery, connection))
        {
            deleteCommand.Transaction = transaction;
            int deleteRowsAffected = deleteCommand.ExecuteNonQuery();
            if (deleteRowsAffected <= 0)
                throw new Exception("Failed to delete post.");

            Console.WriteLine("Post deleted successfully.");
        }
    }


    public bool DeletePost(string postId, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("Posts", "PostId", postId))
                    throw new Exception($"Post {postId} not found.");

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        innerDeletePost(connection, transaction, postId, channelId);
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting post with ID {postId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool DeleteRating(string ratingId, string channelId, string postId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Channels", "ChannelId", channelId))
                    throw new Exception($"Channel {channelId} not found.");
                
                if(!isInDataBase("Posts", "PostId", postId))
                    throw new Exception($"Post {postId} not found.");

                if(!isInDataBase("Ratings", "RatingId", ratingId))
                    throw new Exception($"Rating {ratingId} not found.");
                

                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteRatingFromPostImagesQuery = @$"UPDATE Posts p
                            SET p.Ratings = 
                                CAST(
                                    MULTISET(
                                        SELECT pr.COLUMN_VALUE
                                        FROM TABLE(p.Ratings) pr
                                        WHERE pr.COLUMN_VALUE != (SELECT REF(r) FROM Ratings r WHERE r.RatingId = '{ratingId}')
                                    ) AS RatingsList
                                )
                            WHERE p.PostId = '{postId}'";

                        using (var command = new OracleCommand(deleteRatingFromPostImagesQuery, connection))
                        {

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                                throw new Exception("Failed to delete rating from post.");

                            Console.WriteLine("Rating from post deleted successfully.");
                    
                        }

                        string deleteRatingQuery = @$"
                            DELETE FROM Ratings r
                            WHERE r.RatingId = '{ratingId}'";

                        using (var deleteCommand = new OracleCommand(deleteRatingQuery, connection))
                        {

                            int deleteRowsAffected = deleteCommand.ExecuteNonQuery();
                            if (deleteRowsAffected <= 0)
                                throw new Exception("Failed to delete rating.");

                            Console.WriteLine("Rating deleted successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting rating with ID {ratingId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    private List<(string PostId, string ChannelId)> GetPostsAndChannelsByUserId(string userId)
    {
        List<(string PostId, string ChannelId)> postChannelList = new List<(string PostId, string ChannelId)>();

        using (OracleConnection connection = new OracleConnection(_connectionString))
        {
            connection.Open();

            string query = @$"
                SELECT p.PostId, DEREF(p.ChannelRef).ChannelId AS ChannelId
                FROM Posts p
                WHERE DEREF(p.ChannelRef).UserRef = (SELECT REF(u) FROM Users u WHERE u.UserId = {userId})";

            using (OracleCommand command = new OracleCommand(query, connection))
            {
                using (OracleDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string postId = reader.GetString(0);
                        string channelId = reader.GetString(1);
                        postChannelList.Add((postId, channelId));
                    }
                }
            }
        }

        return postChannelList;
    }

    private List<(string RatingId, string PostId, string ChannelId)> GetRatingsPostsAndChannelsByUserId(string userId)
    {
        List<(string RatingId, string PostId, string ChannelId)> ratingPostChannelList = new List<(string RatingId, string PostId, string ChannelId)>();

        using (OracleConnection connection = new OracleConnection(_connectionString))
        {
            connection.Open();

            string query = @$"
            SELECT r.RatingId RatingId, 
                p.PostId PostId, 
                p.ChannelRef.ChannelId ChannelId
            FROM Posts p, 
                TABLE(p.Ratings) r_list,
                Ratings r
            WHERE r_list.COLUMN_VALUE = REF(r) AND p.ChannelRef.UserRef.UserId='{userId}';";

            using (OracleCommand command = new OracleCommand(query, connection))
            {
                command.Parameters.Add(new OracleParameter("UserId", userId));

                using (OracleDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ratingId = reader.GetString(0);
                        string postId = reader.GetString(1);
                        string channelId = reader.GetString(2);
                        ratingPostChannelList.Add((ratingId, postId, channelId));
                    }
                }
            }
        }

        return ratingPostChannelList;
    }

    public bool DeleteUser(string userId)
    {
        try
        {
            List<string> channelIds = GetIdsByuser(userId, "Channels", "ChannelId", "UserRef");
            //List<string> messagesIds = GetIdsByuser(userId, "Messages", "MessageId", "Sender");
            //List<(string RatingId, string PostId, string ChannelId)> ratingsIdsWithPostAndChannelId = GetRatingsPostsAndChannelsByUserId(userId);

            using (var connection = new OracleConnection(_connectionString))
            {

                if(!isInDataBase("Users", "UserId", userId))
                    throw new Exception($"User {userId} not found.");
                
                connection.Open();                   
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (string channelId in channelIds)
                        {
                            Console.WriteLine(channelId);
                            innerDeleteChannel(connection,transaction,channelId);
                        }


                        string deleteMessagesQuery = @$"
                            DELETE FROM Messages m
                            WHERE m.MessageId IN (SELECT m.MessageId
                                FROM Messages m
                                WHERE DEREF(m.Sender).UserId = '{userId}')";

                        Console.WriteLine(deleteMessagesQuery);

                        using (var deleteCommand = new OracleCommand(deleteMessagesQuery, connection))
                        {
                            deleteCommand.Transaction = transaction;
                            deleteCommand.ExecuteNonQuery();
                        }
                    

                        // foreach ((string ratingId, string postId, string channelId) in ratingsIdsWithPostAndChannelId)
                        // {
                        //     if(!DeleteRating(ratingId, postId, channelId))
                        //         throw new Exception($"Delete rating {ratingId} error");
                        // }

                        string deleteRatingsQuery = @$"
                            DELETE FROM Ratings r
                            WHERE r.RatingId IN (SELECT r.RatingId
                                FROM Ratings r
                                WHERE DEREF(r.Author).UserId = '{userId}')";

                        Console.WriteLine(deleteRatingsQuery);

                        using (var deleteCommand = new OracleCommand(deleteMessagesQuery, connection))
                        {
                            deleteCommand.Transaction = transaction;
                            deleteCommand.ExecuteNonQuery();
                        }

                        string deleteUserQuery = @$"
                            DELETE FROM Users u
                            WHERE u.UserId = '{userId}'";

                        Console.WriteLine(deleteUserQuery);

                        using (var deleteCommand = new OracleCommand(deleteUserQuery, connection))
                        {
                            deleteCommand.Transaction = transaction;
                            int deleteRowsAffected = deleteCommand.ExecuteNonQuery();
                            if (deleteRowsAffected <= 0)
                                throw new Exception("Failed to delete user.");

                            Console.WriteLine("User deleted successfully.");
                        }
                        transaction.Commit();
                        Console.WriteLine("Transaction committed successfully.");

                    } 
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error: {ex.Message}. Transaction rolled back.");
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user with ID {userId}. \nException Message: {ex.Message}");
            return false;
        }
    }


    public Channel GetChannel(string channelId)
    {
        try
        {
            Channel channel = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"SELECT c.ChannelId, c.ChannelName, c.ChannelDescription, c.DateOfCreated, 
                    c.UserRef.UserId OwnerId FROM Channels c WHERE c.ChannelId = '{channelId}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            channel = new Channel
                            {
                                Id = reader.GetString(reader.GetOrdinal("ChannelId")),
                                Name = reader.GetString(reader.GetOrdinal("ChannelName")),
                                Description = reader.GetString(reader.GetOrdinal("ChannelDescription")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("DateOfCreated")),
                                OwnerId = reader.GetString(reader.GetOrdinal("OwnerId"))
                            };
                        }
                    }
                }
            }

            if (channel == null)
            {
                Console.WriteLine($"Channel with ID {channelId} not found.");
            }

            return channel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving channel with ID {channelId}. \nException Message: {ex.Message}");
            return null;
        }
    }

    public List<Channel> GetChannels()
    {
        try
        {
            var channels = new List<Channel>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT c.ChannelId, c.ChannelName, c.ChannelDescription, c.DateOfCreated, 
                c.UserRef.UserId OwnerId FROM Channels c";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var channel = new Channel
                            {
                                Id = reader.GetString(reader.GetOrdinal("ChannelId")),
                                Name = reader.GetString(reader.GetOrdinal("ChannelName")),
                                Description = reader.GetString(reader.GetOrdinal("ChannelDescription")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("DateOfCreated")),
                                OwnerId = reader.GetString(reader.GetOrdinal("OwnerId"))
                            };

                            channels.Add(channel);
                        }
                    }
                }
            }

            return channels;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving channels. \nException Message: {ex.Message}");
            return new List<Channel>();
        }
    }

    public List<Message> GetMessagesFrom(string userId)
    {
        try
        {
            var messages = new List<Message>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                // Zapytanie SQL, które pobiera wiadomości wysłane przez użytkownika
                string query = @$"
                    SELECT MessageId, MessageContent, DateTimeOfSent, 
                    DEREF(Sender).UserId SenderId, DEREF(Receiver).UserId ReceiverId
                    FROM Messages
                    WHERE DEREF(Sender).UserId = '{userId}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new Message
                            {
                                Id = reader.GetString(reader.GetOrdinal("MessageId")),
                                Content = reader.GetString(reader.GetOrdinal("MessageContent")),
                                Date = reader.GetDateTime(reader.GetOrdinal("DateTimeOfSent")),
                                SenderId = reader.GetString(reader.GetOrdinal("SenderId")),
                                RecipientId = reader.GetString(reader.GetOrdinal("ReceiverId"))
                            };

                            message.Sender = GetUser(message.SenderId);
                            message.Recipient = GetUser(message.RecipientId);

                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving messages from user with ID {userId}. \nException Message: {ex.Message}");
            return new List<Message>();
        }
    }

    public List<Message> GetMessagesTo(string userId)
    {
        try
        {
            var messages = new List<Message>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"
                    SELECT MessageId, MessageContent, DateTimeOfSent, 
                    DEREF(Sender).UserId SenderId, DEREF(Receiver).UserId ReceiverId
                    FROM Messages
                    WHERE DEREF(Receiver).UserId = '{userId}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new Message
                            {
                                Id = reader.GetString(reader.GetOrdinal("MessageId")),
                                Content = reader.GetString(reader.GetOrdinal("MessageContent")),
                                Date = reader.GetDateTime(reader.GetOrdinal("DateTimeOfSent")),
                                SenderId = reader.GetString(reader.GetOrdinal("SenderId")),
                                RecipientId = reader.GetString(reader.GetOrdinal("ReceiverId"))
                            };
                            message.Sender = GetUser(message.SenderId);
                            message.Recipient = GetUser(message.RecipientId);

                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving messages to user with ID {userId}. \nException Message: {ex.Message}");
            return new List<Message>();
        }
    }

    public Post GetPost(string postId, string channelId)
    {
        try
        {
            Post post = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"SELECT PostId, Title, PostContent, CreationDate, IsSponsored 
                    FROM Posts WHERE PostId = '{postId}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = new Post
                            {
                                Id = reader.GetString(reader.GetOrdinal("PostId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Content = reader.GetString(reader.GetOrdinal("PostContent")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                IsSponsored = reader.GetBoolean(reader.GetOrdinal("IsSponsored"))
                            };
                        }
                    }
                }
            }

            if (post == null)
            {
                Console.WriteLine($"Post with ID {postId} not found.");
            }

            return post;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving port with ID {postId}. \nException Message: {ex.Message}");
            return null;
        }
    }

    public User GetUser(string userId)
    {
        try
        {
            User user = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"SELECT u.UserId, u.IsVerified, u.DateOfRegister,
                 u.ProfileObj.FirstName FirstName, u.ProfileObj.Surname Surname,
                  u.ProfileObj.Email Email FROM Users u WHERE u.UserId = '{userId}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    cmd.Parameters.Add(new OracleParameter("userId", userId));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetString(reader.GetOrdinal("UserId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("Surname")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("DateOfRegister")),
                                IsVerified = reader.GetBoolean(reader.GetOrdinal("IsVerified"))
                            };
                        }
                    }
                }
            }

            if (user == null)
            {
                Console.WriteLine($"User with ID {userId} not found.");
            }

            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user with ID {userId}. \nException Message: {ex.Message}");
            return null;
        }
    }

    public List<User> GetUsers()
    {
        try
        {
            var users = new List<User>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT u.UserId, u.IsVerified, u.DateOfRegister,
                 u.ProfileObj.FirstName FirstName, u.ProfileObj.Surname Surname,
                  u.ProfileObj.Email Email FROM Users u";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                Id = reader.GetString(reader.GetOrdinal("UserId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("Surname")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("DateOfRegister")),
                                IsVerified = reader.GetBoolean(reader.GetOrdinal("IsVerified"))
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving users. \nException Message: {ex.Message}");
            return null;
        }
    }

    public bool UpdateChannel(Channel channel)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"
                    UPDATE Channels
                    SET ChannelName = '{channel.Name}',
                        ChannelDescription = '{channel.Description}',
                        UserRef = (SELECT REF(u) FROM Users u WHERE u.UserId = '{channel.OwnerId}')
                    WHERE ChannelId = '{channel.Id}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected<=0)
                        throw new Exception($"No rows were updated for channel ID {channel.Id}.");
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating channel with ID {channel.Id}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool UpdateLiveStream(LiveStream stream, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                string endDateString = stream.EndDate.ToString("yyyy-MM-dd HH:mm:ss");

                string query = @$"
                    UPDATE Streams
                    SET StreamName = '{stream.Name}',
                        SavePath = '{stream.SavedPath}',
                        DateOfEnd = TO_DATE('{endDateString}', 'YYYY-MM-DD HH24:MI:SS')
                    WHERE StreamId = '{stream.Id}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected<=0)
                        throw new Exception($"No rows were updated for stream ID {stream.Id}.");
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating LiveStream with ID {stream.Id} in channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool UpdatePost(Post post, string channelId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string isSponsoredString = post.IsSponsored ? "1" : "0";
                string query = @$"
                    UPDATE Posts
                    SET Title = '{post.Title}',
                        PostContent = '{post.Content}',
                        IsSponsored = '{isSponsoredString}'
                    WHERE PostId = '{post.Id}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected<=0)
                        throw new Exception($"No rows were updated for post ID {post.Id}.");
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating post with ID {post.Id} in channel {channelId}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool UpdateUser(User user)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"
                    UPDATE Users
                        SET ProfileObj = UserProfile('{user.FirstName}', '{user.LastName}', '{user.Email}')
                    WHERE UserId = '{user.Id}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected<=0)
                        throw new Exception($"No rows were updated for user ID {user.Id}.");
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user with ID {user.Id}. \nException Message: {ex.Message}");
            return false;
        }
    }

    public bool VerifyUser(string userId)
    {
        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                string query = @$"
                    UPDATE Users
                        SET IsVerified = '1'
                    WHERE UserId = '{userId}'";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected<=0)
                        throw new Exception($"No rows were updated for user ID {userId}.");
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying user with ID {userId}. \nException Message: {ex.Message}");
            return false;
        }
    }

}