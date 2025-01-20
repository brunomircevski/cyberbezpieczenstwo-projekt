using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BDwAS_projekt.Models;
using BDwAS_projekt.Models.Dto;
using BDwAS_projekt.Data;
using MongoDB.Bson;

namespace BDwAS_projekt.Controllers;

[ApiController]
[Route("Test")]
public class TestController : Controller
{
    private readonly IDbContext _db;
    private static readonly Random Random = new();


    public TestController(IDbContext db)
    {
        _db = db;
    }

    [HttpPost("Messages")]
    public IActionResult PostMessages(int number)
    {
        var messages = GenerateMessages(number);

        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach (var message in messages)
        {
            _db.AddMessage(message);
        }
        stopwatch.Stop();

        return Ok(new { Message = "Added " + number + " random messages.", TimeMiliseconds = stopwatch.ElapsedMilliseconds });
    }

    [HttpPost("Users")]
    public IActionResult PostUsers(int number)
    {
        var users = GenerateUsers(number);

        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach (var user in users)
        {
            _db.AddUser(user);
        }
        stopwatch.Stop();

        return Ok(new { Message = "Added " + number + " random users.", TimeMiliseconds = stopwatch.ElapsedMilliseconds });
    }

    [HttpPost("ChannelPostComment")]
    public IActionResult PostChannelPostComment(int channels, int posts, int comments)
    {
        var channelsList = GenerateChannels(channels);
        var postsList = GeneratePosts(channelsList, posts);
        var commentsList = GenerateComments(postsList, comments);

        Stopwatch stopwatch = Stopwatch.StartNew();

        foreach (var channel in channelsList)
        {
            _db.AddChannel(channel);
        }

        foreach (var post in postsList)
        {
            _db.AddPost(post.Post, post.ChannelId);
        }

        foreach (var comment in commentsList)
        {
            _db.AddComment(comment.Comment, comment.ChannelId, comment.PostId);
        }

        stopwatch.Stop();

        return Ok(new
        {
            Message = $"Added {channels} channels with {posts} posts each, with {comments} comments each.",
            TimeMilliseconds = stopwatch.ElapsedMilliseconds
        });
    }

    [HttpPut("Users")]
    public IActionResult UpdateUsers(int number)
    {
        var users = _db.GetUsers();
        int actualCount = Math.Min(number, users.Count);

        // Pregenerate email updates
        var randomEmails = new List<string>();
        for (int i = 0; i < actualCount; i++)
        {
            randomEmails.Add($"{GenerateRandomWord()}@example.com");
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < actualCount; i++)
        {
            users[i].Email = randomEmails[i];
            _db.UpdateUser(users[i]);
        }

        stopwatch.Stop();

        return Ok(new
        {
            Message = $"Updated {actualCount} users' emails.",
            TimeMilliseconds = stopwatch.ElapsedMilliseconds
        });
    }

    [HttpDelete("Users")]
    public IActionResult DeleteUsers(int number)
    {
        var users = _db.GetUsers();
        int actualCount = Math.Min(number, users.Count);

        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < actualCount; i++)
        {
            _db.DeleteUser(users[i].Id);
        }

        stopwatch.Stop();

        return Ok(new
        {
            Message = $"Deleted {actualCount} users.",
            TimeMilliseconds = stopwatch.ElapsedMilliseconds
        });
    }


    [HttpGet("Users")]
    public IActionResult GetUsers()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var users = _db.GetUsers();
        stopwatch.Stop();

        if (users.Where(u => u.FirstName == "Adolf" && u.LastName == "Hitler").FirstOrDefault() != null)
            throw new Exception("Nie wiedział o holocauście");

        return Ok(new { Message = "Got " + users.Count + " users.", TimeMiliseconds = stopwatch.ElapsedMilliseconds });
    }

    private static List<User> GenerateUsers(int number)
    {
        var users = new List<User>();

        for (int i = 0; i < number; i++)
        {
            string firstName = GenerateRandomWord();
            string lastName = GenerateRandomWord();
            string email = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";

            users.Add(new User
            {
                Id = GenerateRandomId(24),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                RegistrationDate = DateTime.Now.AddDays(-Random.Next(0, 3650)), // Random time within the last 10 years
                IsVerified = Random.Next(0, 2) == 1 // Randomly true or false
            });
        }

        return users;
    }

    private static List<Message> GenerateMessages(int number)
    {
        var messages = new List<Message>();

        for (int i = 0; i < number; i++)
        {
            messages.Add(new Message
            {
                Id = GenerateRandomId(24),
                Content = GenerateRandomContent(),
                Date = DateTime.Now.AddMinutes(-Random.Next(0, 100000)), // Random time within ~70 days
                SenderId = GenerateRandomId(24),
                RecipientId = GenerateRandomId(24)
            });
        }

        return messages;
    }

    private static string GenerateRandomId(int length)
    {
        const string chars = "abcde0123456789";
        var result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[Random.Next(chars.Length)];
        }

        return new string(result);
    }

    private static string GenerateRandomContent()
    {
        string[] sampleMessages =
        {
            "Hello, how are you?",
            "This is a random message.",
            "Let's meet tomorrow at 5 PM.",
            "Did you finish the task?",
            "Happy birthday!",
            "See you at the party tonight!",
            "I will call you later.",
            "Check out this new app.",
            "Don't forget about the meeting.",
            "I'll send the files shortly.",
            "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
            "It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
            "It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like)."
        };

        return sampleMessages[Random.Next(sampleMessages.Length)];
    }

    private static string GenerateRandomWord()
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int wordLength = Random.Next(6, 21);
        char[] word = new char[wordLength];

        for (int i = 0; i < wordLength; i++)
        {
            word[i] = characters[Random.Next(characters.Length)];
        }

        return new string(word);
    }

    // Generate a list of random Channels
    private List<Channel> GenerateChannels(int count)
    {
        var channels = new List<Channel>();
        for (int i = 0; i < count; i++)
        {
            channels.Add(new Channel
            {
                Id = GenerateRandomId(24),
                Name = GenerateRandomWord(),
                Description = GenerateRandomWord(),
                CreationDate = DateTime.Now.AddDays(-new Random().Next(0, 365)),
                OwnerId = GenerateRandomId(24),
                Posts = []
            });
        }
        return channels;
    }

    // Generate a list of random Posts for each Channel
    private List<(Post Post, string ChannelId)> GeneratePosts(List<Channel> channels, int count)
    {
        var posts = new List<(Post Post, string ChannelId)>();
        foreach (var channel in channels)
        {
            for (int i = 0; i < count; i++)
            {
                posts.Add((
                    new Post
                    {
                        Id = GenerateRandomId(24),
                        Title = GenerateRandomWord(),
                        Content = GenerateRandomContent(),
                        CreationDate = DateTime.Now.AddDays(-new Random().Next(0, 365)),
                        IsSponsored = new Random().Next(0, 2) == 1,
                        Comments = []
                    },
                    channel.Id
                ));
            }
        }
        return posts;
    }

    // Generate a list of random Comments for each Post
    private List<(Comment Comment, string ChannelId, string PostId)> GenerateComments(List<(Post Post, string ChannelId)> posts, int count)
    {
        var comments = new List<(Comment Comment, string ChannelId, string PostId)>();
        foreach (var post in posts)
        {
            for (int i = 0; i < count; i++)
            {
                comments.Add((
                    new Comment
                    {
                        Id = GenerateRandomId(24),
                        Content = GenerateRandomContent(),
                        CreationDate = DateTime.Now.AddMinutes(-new Random().Next(0, 100000)),
                        AuthorId = GenerateRandomId(24)
                    },
                    post.ChannelId,
                    post.Post.Id
                ));
            }
        }
        return comments;
    }
}
