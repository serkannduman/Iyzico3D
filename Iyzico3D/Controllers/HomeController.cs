using Microsoft.AspNetCore.Mvc;

namespace Iyzico3D.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
