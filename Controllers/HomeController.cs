using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BDwAS_projekt.Models;
using BDwAS_projekt.Data;

namespace BDwAS_projekt.Controllers;

public class HomeController : Controller
{
    private readonly IDbContext _db;

    public HomeController(IDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }
}
