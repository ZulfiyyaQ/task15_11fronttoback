using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using task15_11fronttoback.Areas.Admin.ViewModels;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Include(x => x.Category).Include(p => p.ProductImages.Where(p => p.IsPrimary == true)).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductsVM productvm)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View();
            }


            bool result = await _context.Categories.AnyAsync(c => c.Id == productvm.CategoryId);
            if (!result)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bele Id li category movcud deyil");
                return View();
            }
            Product product = new Product
            {
                Name = productvm.Name,
                Price = productvm.Price,
                SKU = productvm.SKU,
                CategoryId = (int)productvm.CategoryId,
                Description = productvm.Description
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //ViewBag.Categories = await _context.Categories.ToListAsync();
            //return View();

        }
        //public async Task<IActionResult> Update(int id)
        //{
        //    if (id <= 0) return BadRequest();
        //    Product existed = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
        //    if (existed is null)  return NotFound(); 
        //    return View();
        //    UpdateProductVM productvm = new UpdateProductVM 
        //    {
        //        Name = existed.Name,
        //        Price = existed.Price,
        //        SKU = existed.SKU,
        //        CategoryId = (int)existed.CategoryId,
        //        Description = existed.Description
        //    };
        //    return View(productvm);

        //}
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = existed.Name,
                Price = existed.Price,
                Description = existed.Description,
                SKU = existed.SKU,
                CategoryId = (int)existed.CategoryId
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(productVM);
            }

            Product existed = await _context.Products.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();

            bool result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Name", "Product already exists");
                return View(productVM);
            }

            bool result1 = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result1)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("CategoryId", "Category not found, choose another one.");
                return View(productVM);
            }

            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = (int)productVM.CategoryId,
                Description = productVM.Description
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //[HttpPost]
        //public async Task<IActionResult> Update(int id, UpdateProductVM productvm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    Product existed = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
        //    if (existed is null) return NotFound();
        //    bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == productvm.Name.ToLower().Trim() && c.Id != id);
        //    if (result)
        //    {
        //        ModelState.AddModelError("Name", "Bele product artiq movcutdur");
        //        return View();
        //    }

        //    existed.Name = productvm.Name;
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));

        //}

    }
}
