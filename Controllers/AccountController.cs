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
    private readonly LogService _logService;


    public AccountController(SQLiteContext db, AccountService accountService, LogService logService)
    {
        _db = db;
        _accountService = accountService;
        _logService = logService;
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
        User user = _db.Users.Where(u => u.Username == username).FirstOrDefault();
        if (user is null)
        {
            _logService.LogNonExistingUserLoginAttempt(username);
            return BadRequest(new { Message = "Invalid login or password" });
        }

        if (user.IsLocked)
        {
            return BadRequest(new { Message = "Account locked!" });
        }

        if (user.FailedLoginsCounter > 0 && user.LastFailedLogin.HasValue)
        {
            int lockoutTime = user.FailedLoginsCounter * 10; // liczba nieudanych prób * 10 sekund
            TimeSpan timeSinceLastFailed = DateTime.Now - user.LastFailedLogin.Value;

            if (timeSinceLastFailed.TotalSeconds < lockoutTime)
            {
                int remainingTime = lockoutTime - (int)timeSinceLastFailed.TotalSeconds;
                return BadRequest(new { Message = $"Try again in {remainingTime} seconds." });
            }
        }

        if (user.Password != password)
        {
            //User found, password incorrect
            user.LastFailedLogin = DateTime.Now;

            _logService.LogFailedLoginWithCounter(username, user.FailedLoginsCounter, user.LastFailedLogin.Value);

            if (user.MaxFailedLogins == ++user.FailedLoginsCounter)
            {
                user.IsLocked = true;
            }

            _db.SaveChanges();

            return BadRequest(new { Message = "Invalid login or password" });
        }

        user.LastLogin = DateTime.Now;
        user.FailedLoginsCounter = 0;

        _logService.LogSuccessfulLogin(username, user.LastLogin.Value);

        _db.SaveChanges();

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

    [HttpPost("maxFailedLogins")]
    public IActionResult SetMaxFailedLogins(string token, int num)
    {
        if (String.IsNullOrEmpty(token)) return NotFound(new { Message = "Invalid token!" });
        int userId = _accountService.GetUserId(token);
        if (userId == 0) return NotFound(new { Message = "Invalid token!" });

        User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (user is null) return NotFound(new { Message = "User not found!" });

        user.MaxFailedLogins = num;
        _db.SaveChanges();

        return Ok(user);
    }
}
