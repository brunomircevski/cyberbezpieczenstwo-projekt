using Microsoft.AspNetCore.Mvc;

namespace Cyberbezpieczenstwo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("/swagger");
    }
}
