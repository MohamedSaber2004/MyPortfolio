using Microsoft.AspNetCore.Mvc;

namespace MyPortfolio.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
