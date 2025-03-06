using Microsoft.AspNetCore.Mvc;
using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Data;
using Cyberbezpieczenstwo.Services;

namespace Cyberbezpieczenstwo.Controllers;

[ApiController]
[Route("message")]
public class MessageController : Controller
{
    private readonly SQLiteContext _db;
    private readonly AccountService _accountService;

    public MessageController(SQLiteContext db, AccountService accountService)
    {
        _db = db;
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var messages = _db.Messages.ToList();
        // return Ok(new List<Message>([new Message() { Id = 1, Content = "Nie ma absolutnie żadncyh dowódów, aby A...", Date = DateTime.Now, Sender = new User() { Id = 1, Username = "JKM" } }]));
        return Ok(messages);

    }

    [HttpPost]
    public IActionResult Send(string token, string message)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int userId = _accountService.GetUserId(token);
        if (userId == 0) return NotFound(new { Message = "Invalid token!" });

        User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (user is null) return NotFound(new { Message = "User not found!" });

        int newId = _db.Messages.Any() ? _db.Messages.Max(m => m.Id) + 1 : 1;
        Message newMessage = new Message() { Id = newId, Content = message, Date = DateTime.Now, Sender = user, SenderId = userId };

        _db.Messages.Add(newMessage);
        _db.SaveChanges();

        return Ok(newMessage);
    }

    [HttpPut("/{messageId}")]
    public IActionResult Update(string token, int messageId, string newMessage)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int userId = _accountService.GetUserId(token);
        if (userId == 0) return NotFound(new { Message = "Invalid token!" });

        Message message = _db.Messages.Where(m => m.Id == messageId).FirstOrDefault();
        if (message is null) return NotFound(new { Message = "Message not found!" });
        if (message.SenderId != userId) return NotFound(new { Message = "Invalid token!" });

        message.Content = newMessage;
        _db.Update(message);
        _db.SaveChanges();

        return Ok(message);
    }

    [HttpDelete("/{messageId}")]
    public IActionResult Delete(string token, int messageId)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int userId = _accountService.GetUserId(token);
        if (userId == 0) return NotFound(new { Message = "Invalid token!" });

        Message message = _db.Messages.Where(m => m.Id == messageId).FirstOrDefault();
        if (message is null) return NotFound(new { Message = "Message not found!" });
        if (message.SenderId != userId) return NotFound(new { Message = "Invalid token!" });

        _db.Messages.Remove(message);
        _db.SaveChanges();

        return Ok();
    }
}
