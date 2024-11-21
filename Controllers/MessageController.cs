using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BDwAS_projekt.Models;
using BDwAS_projekt.Models.Dto;
using BDwAS_projekt.Data;
using MongoDB.Bson;

namespace BDwAS_projekt.Controllers;

[ApiController]
[Route("Message")]
public class MessageController : Controller
{
    private readonly IDbContext _db;

    public MessageController(IDbContext db)
    {
        _db = db;
    }

    [HttpGet("To/{userId}")]
    public IActionResult GetMessagesTo(string userId)
    {
        if (userId.Length != 24) return BadRequest("Id must be 24 characters long.");
        List<Message> messages = _db.GetMessagesTo(userId);

        return Ok(messages);
    }

    [HttpGet("From/{userId}")]
    public IActionResult GetMessagesFrom(string userId)
    {
        if (userId.Length != 24) return BadRequest("Id must be 24 characters long.");
        List<Message> messages = _db.GetMessagesFrom(userId);

        return Ok(messages);
    }

    [HttpPost("")]
    public IActionResult AddMessage([FromBody] MessageDto messageDto)
    {
        if (messageDto == null) return BadRequest("Message data is required.");

        Message message = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Content = messageDto.Content,
            SenderId = messageDto.SenderId,
            RecipientId = messageDto.RecipientId,
            Date = DateTime.Now
        };

        if (_db.AddMessage(message)) return Ok(message);
        else return BadRequest("Failed to add message.");
    }

    [HttpDelete("{messageId}")]
    public IActionResult DeleteMessage(string messageId)
    {
        if (messageId.Length != 24) return BadRequest("Id must be 24 characters long.");

        if (_db.DeleteMessage(messageId)) return Ok("Message deleted.");
        else return NotFound("Failed to delete messageId.");
    }
}
