using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;


        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Product> products = _context.Products.OrderByDescending(p => p.Id).Take(8).ToList();

            HomeVM vm = new()
            {
                Products = products,
            };

            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
