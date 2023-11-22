using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController (AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        
        public async Task<IActionResult> Create(Category categories)
        {
            if (!ModelState.IsValid) return View();

            bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == categories.Name.ToLower().Trim());
            if(result)
            {
                ModelState.AddModelError("Name", "Bele category artiq movcutdur");
                return View();
            }

            await _context.Categories.AddAsync(categories);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Update(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id );
            if (existed is null) return NotFound();
            bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Category already exists");
                return View();
            }
            existed.Name = category.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            _context.Categories.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
        public async Task<IActionResult> Detail(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).ThenInclude(p => p.ProductImages).FirstOrDefaultAsync(pi => pi.Id == id);
            return View(category);
        }

    }
}
