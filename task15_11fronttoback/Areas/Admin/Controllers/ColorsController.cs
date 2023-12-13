using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.Areas.Admin.ViewModels;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]

    public class ColorsController : Controller
    {
        private readonly AppDbContext _context;

        public ColorsController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator")]


        public async Task<IActionResult> Index(int page=1)
        {
            double count = await _context.Categories.CountAsync();

            List<Color> colors = await _context.Colors.Skip((page - 1) * 3).Take(3)
                .Include(x=>x.ProductColors).ThenInclude(p=>p.Product).ToListAsync();
            PaginationVM<Color> paginateVM = new PaginationVM<Color>
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 3),
                Items = colors

            };
            return View(paginateVM);
           
        }



        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Create(CreateColorsVM colorvm)
        {
            if (!ModelState.IsValid) return View();

            bool result = _context.Colors.Any(c => c.Name.ToLower().Trim() == colorvm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele reng artiq movcutdur");
                return View();
            }
            Color color = new Color
            { 
                Name=colorvm.Name
            };

            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            Color existed = await _context.Colors.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateColorsVM vm = new()
            {
                Name = existed.Name,

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorsVM colorvm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

         Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Colors.Any(c => c.Name.ToLower().Trim() == colorvm.Name.ToLower().Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele reng artiq movcutdur");
                return View();
            }
            existed.Name = colorvm.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            _context.Colors.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
        public async Task<IActionResult> Detail(int id)
        {
            var color = await _context.Colors.Include(c => c.ProductColors).ThenInclude(p => p.Product)/*.ThenInclude(pi=>pi.ProductImages)*/.FirstOrDefaultAsync(pi => pi.Id == id);
            if (color is null) return NotFound();
            return View(color);
        }
    }
}
