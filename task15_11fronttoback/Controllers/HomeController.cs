using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.Services;
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
        public async Task<IActionResult> Index()
        {

            List<Slide> slides = _context.Slides.OrderBy(p => p.Order).ToList();
            List<Product> productList = _context.Products.Include(x => x.ProductImages).ToList();

            HomeVM vm = new HomeVM
            {
                Products = productList,
                Slides = slides,
                LatestProducts = productList.OrderByDescending(p => p.Id).Take(8).ToList()
            };
            return View(vm);
        }

         
        public IActionResult About()
        {
            return View();
        }
    }
}
