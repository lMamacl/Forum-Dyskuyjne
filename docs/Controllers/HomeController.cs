using Microsoft.AspNetCore.Mvc;

namespace Forum_Dyskusyjne.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}