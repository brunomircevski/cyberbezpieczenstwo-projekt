using Microsoft.AspNetCore.Mvc;
using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Data;
using Microsoft.EntityFrameworkCore;
using Cyberbezpieczenstwo.Services;

namespace Cyberbezpieczenstwo.Controllers;

[ApiController]
[Route("account")]
public class AccountController : Controller
{
    private readonly SQLiteContext _db;
    private readonly AccountService _accountService;

    public AccountController(SQLiteContext db, AccountService accountService)
    {
        _db = db;
        _accountService = accountService;
    }

    [HttpPost("register")]
    public IActionResult Register(string username, string password)
    {
        if (_db.Users.Any(u => u.Username == username)) return BadRequest(new { Message = "Username already taken!" });

        int newId = _db.Users.Any() ? _db.Users.Max(u => u.Id) + 1 : 1;
        User newUser = new() { Id = newId, Username = username, Password = password };

        _db.Users.Add(newUser);
        _db.SaveChanges();

        return Ok(newUser);
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        User user = _db.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
        if (user is null) return NotFound(new { Message = "User not found!" });

        string token = _accountService.GetToken(user.Id);
        return Ok(new { Message = "Welcome, " + user.Username, Token = token });
    }

    [HttpPost("logout")]
    public IActionResult Logout(string token)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int userId = _accountService.GetUserId(token);
        if (userId == 0) return NotFound(new { Message = "Invalid token!" });

        _accountService.Logout(token);
        return Ok();
    }

    [HttpDelete]
    public IActionResult DeleteAccount(string token)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int userId = _accountService.GetUserId(token);
        if (userId == 0) return NotFound(new { Message = "Invalid token!" });

        User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (user is null) return NotFound(new { Message = "User not found!" });

        _accountService.Logout(token);
        
        _db.Users.Remove(user);
        _db.SaveChanges();

        return Ok();
    }

    [HttpGet("my")]
    public IActionResult GetUser(string token)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });

        int userId = _accountService.GetUserId(token);
        if (userId == 0) return NotFound(new { Message = "Invalid token!" });

        User user = _db.Users
            .Where(u => u.Id == userId)
            .Include(u => u.OwnMessages)
            .Include(u => u.EditableMessages)
            .FirstOrDefault();
        if (user is null) return NotFound(new { Message = "User not found!" });

        return Ok(user);
    }
}
