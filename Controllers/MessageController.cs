using Microsoft.AspNetCore.Mvc;
using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Data;
using Microsoft.EntityFrameworkCore;

namespace Cyberbezpieczenstwo.Controllers;

[ApiController]
[Route("Message")]
public class MessageController : Controller
{
    private readonly SQLiteContext _db;

    public MessageController(SQLiteContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        //Get all messages
        return Ok(new List<Message>([new Message() { Id = 1, Content = "Nie ma absolutnie żadncyh dowódów, aby A...", Date = DateTime.Now, Sender = new User() { Id = 1, Login = "JKM" } }]));
    }

    [HttpPost]
    public IActionResult Send(string token, string message)
    {
        //Create new message, return it with new id
        return Ok(new Message() { Id = 1, Content = "Nie ma absolutnie żadncyh dowódów, aby A...", Date = DateTime.Now, Sender = new User() { Id = 1, Login = "JKM" }});
    }

    [HttpPut("/{messageId}")]
    public IActionResult Update(string token, string messageId, string message)
    {
        //Update existing message by id, return it
        return Ok(new Message() { Id = 1, Content = "Nie ma absolutnie żadncyh dowódów, aby A...", Date = DateTime.Now, Sender = new User() { Id = 1, Login = "JKM" }});
    }

    [HttpDelete("/{messageId}")]
    public IActionResult Delete(string token, string messageId)
    {
        //Delete message by id
        return Ok();
    }
}
