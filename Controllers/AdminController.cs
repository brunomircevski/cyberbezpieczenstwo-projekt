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

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _db.Users.ToList();
        return Ok(users);
    }
}
