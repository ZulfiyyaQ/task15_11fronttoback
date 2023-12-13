using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.Areas.Admin.ViewModels;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]

    public class SizesController : Controller
    {
        private readonly AppDbContext _context;

        public SizesController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Size> Sizes = await _context.Sizes.Include(x => x.ProductSizes).ThenInclude(p => p.Product).ToListAsync();
            return View(Sizes);
        }
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Create(CreateSizesVM sizevm)
        {
            if (!ModelState.IsValid) return View();

            bool result = _context.Sizes.Any(c => c.Name.ToLower().Trim() == sizevm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele size artiq movcutdur");
                return View();
            }
            Size size = new Size 
            {
                Name=sizevm.Name
            };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task< IActionResult> Update(int id)
        {
            Size existed = await _context.Sizes.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateSizesVM vm = new()
            {
                Name = existed.Name,

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSizesVM sizevm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Sizes.Any(c => c.Name.ToLower().Trim() == sizevm.Name.ToLower().Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele size artiq movcutdur");
                return View();
            }
            existed.Name = sizevm.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            _context.Sizes.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
        public async Task<IActionResult> Detail(int id)
        {
            var size = await _context.Sizes.Include(c => c.ProductSizes).ThenInclude(p => p.Product)/*.ThenInclude(pi=>pi.ProductImages)*/.FirstOrDefaultAsync(pi => pi.Id == id);
            if (size is null) return NotFound();
            return View(size);
        }

    }
}
