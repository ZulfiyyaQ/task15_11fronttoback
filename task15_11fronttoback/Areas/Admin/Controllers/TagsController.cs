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

    public class TagsController : Controller
    {

        private readonly AppDbContext _context;
        public TagsController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(t => t.ProductTags).ToListAsync();

            return View(tags);
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagsVM tagvm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Tags.Any(t => t.Name.ToLower().Trim() == tagvm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele Tag artiq mocvutdur");
                return View();
            }

            Tag tag = new Tag 
            { 
                Name=tagvm.Name
            };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task< IActionResult> Update(int id)
        {
            Tag existed = await _context.Tags.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateTagsVM vm = new()
            {
                Name = existed.Name,

            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagsVM tagvm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Tags.Any(t => t.Name.ToLower().Trim() == tagvm.Name.ToLower().Trim() && t.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele Tag artiq movcutdur");
                return View();
            }
            existed.Name = tagvm.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existed is null) return NotFound();

            _context.Tags.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Detail(int id)
        {
            var tag = await _context.Tags.Include(c => c.ProductTags).ThenInclude(p => p.Product)/*.ThenInclude(p => p.ProductImages)*/.FirstOrDefaultAsync(pi => pi.Id == id);
            if (tag is null) return NotFound();
            return View(tag);
        }

    }
}
