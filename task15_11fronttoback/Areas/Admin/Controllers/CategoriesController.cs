using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.Areas.Admin.ViewModels;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController (AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index(int page=1)
        {
            double count = await _context.Categories.CountAsync();


            List<Category> categories = await _context.Categories.Skip((page - 1) * 3).Take(3)
                .Include(c => c.Products).ToListAsync();

            PaginationVM<Category> paginateVM = new PaginationVM<Category>
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 3),
                Items = categories

            };
            return View(paginateVM);
        }



        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Create(CreateCategoriesVM categoryvm)
        {
            if (!ModelState.IsValid) return View();

            bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == categoryvm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele category artiq movcutdur");
                return View();
            }

            Category category = new Category
            {
                Name = categoryvm.Name
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            Category existed = await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateCategoriesVM vm = new()
            {
                Name = existed.Name,

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoriesVM categoryvm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == categoryvm.Name.ToLower().Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele category artiq movcutdur");
                return View();
            }

            existed.Name = categoryvm.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
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
            if (category is null) return NotFound();
            return View(category);
        }

    }
}
