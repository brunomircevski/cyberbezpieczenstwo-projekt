using Microsoft.AspNetCore.Mvc;
using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Data;
using Microsoft.EntityFrameworkCore;
using Cyberbezpieczenstwo.Services;

namespace Cyberbezpieczenstwo.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : Controller
{
    private readonly SQLiteContext _db;
    private readonly AccountService _accountService;

    public AdminController(SQLiteContext db, AccountService accountService)
    {
        _db = db;
        _accountService = accountService;
    }

    [HttpPost("unlock")]
    public IActionResult Unblock(string username)
    {
        User user = _db.Users.Where(u => u.Username == username).FirstOrDefault();
        if(user is null) return NotFound( new { Message = "User not found!"});

        user.IsLocked = false;
        user.FailedLoginsCounter = 0;

        _db.SaveChanges();

        return Ok(user);
    }
}
