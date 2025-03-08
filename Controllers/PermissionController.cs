using Cyberbezpieczenstwo.Data;
using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cyberbezpieczenstwo.Controllers;

[ApiController]
[Route("permission")]
public class PermissionController : Controller
{
    private readonly SQLiteContext _db;
    private readonly AccountService _accountService;

    public PermissionController(SQLiteContext db, AccountService accountService)
    {
        _db = db;
        _accountService = accountService;
    }

    [HttpPost("message/grant")]
    public IActionResult GrantPermission(string token, int messageId, int userId)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int ownerId = _accountService.GetUserId(token);
        if (ownerId == 0) return NotFound(new { Message = "Invalid token!" });
        if (ownerId == userId) return BadRequest("You cannot grant permissions to yourself.");

        Message message = _db.Messages
            .Where(m => m.Id == messageId)
            .Include(m => m.Editors)
            .FirstOrDefault();

        if (message == null) return NotFound("Message does not exist.");
        if (message.SenderId != ownerId) return StatusCode(403, "You are not the owner of this message.");

        User targetUser = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (targetUser == null) return NotFound("User does not exist.");

        if (!message.Editors.Contains(targetUser))
        {
            message.Editors.Add(targetUser);
            _db.SaveChanges();
            return Ok("Permissions granted.");
        }

        return BadRequest("User already have permissions.");
    }

    [HttpPost("message/revoke")]
    public IActionResult RevokePermission(string token, int messageId, int userId)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int ownerId = _accountService.GetUserId(token);
        if (ownerId == 0) return NotFound(new { Message = "Invalid token!" });
        if (ownerId == userId) return BadRequest("You cannot revoke permissions from yourself.");

        Message message = _db.Messages
            .Where(m => m.Id == messageId)
            .Include(m => m.Editors)
            .FirstOrDefault();

        if (message == null) return NotFound("Message does not exist.");
        if (message.SenderId != ownerId) return StatusCode(403, "You are not the owner of this message.");

        User targetUser = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (targetUser == null) return NotFound("User does not exist.");

        if (message.Editors.Contains(targetUser))
        {
            message.Editors.Remove(targetUser);
            _db.SaveChanges();
            return Ok("Permissions revoked.");
        }

        return BadRequest("User did not have permissions.");
    }


}
