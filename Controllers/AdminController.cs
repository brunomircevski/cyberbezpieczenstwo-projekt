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

    public AdminController(SQLiteContext db)
    {
        _db = db;
    }

    [HttpPost("toggle")]
    public IActionResult ToggleAdmin(int userId)
    {
        User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
        if(user is null) return NotFound(new { Message = "User not found!" });

        user.IsAdmin = !user.IsAdmin;
        _db.SaveChanges();

        return Ok(user);
    }

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _db.Users.ToList();
        return Ok(users);
    }
}
