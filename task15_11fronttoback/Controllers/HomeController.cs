using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace task15_11fronttoback.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
