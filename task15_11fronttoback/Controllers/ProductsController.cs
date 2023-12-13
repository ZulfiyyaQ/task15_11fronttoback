using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.Utilities.Exceptions;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;


        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Details(int id)
        {
            if (id <= 0) throw new WrongRequestException("Gonderilen sorgu yalnisdir");

            Product product = _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x=>x.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(x => x.ProductColors).ThenInclude(pt => pt.Color)
                .Include(x => x.ProductSizes).ThenInclude(pt => pt.Size)
                .FirstOrDefault(x => x.Id == id);

            if (product is null) throw new NotFoundException("Bele bir mehsul tapilmadi");



            ProductVM productvm = new ProductVM
            {
                Product = product,
                RelatedProducts = _context.Products.Where(p => p.Category.Id == product.CategoryId && p.Id != product.Id).Include(x => x.ProductImages).ToList(),
            };


           
            return View(productvm);
        }



       
    }
}
