using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagsController : Controller
    {

        private readonly AppDbContext _context;
        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(t => t.ProductTags).ToListAsync();

            return View(tags);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Tags.Any(t => t.Name.ToLower().Trim() == tag.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Tag already exists");
                return View();
            }
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  
        }
        public IActionResult Update(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Tags.Any(t => t.Name.ToLower().Trim() == tag.Name.ToLower().Trim() && t.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Category already exists");
                return View();
            }
            existed.Name = tag.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existed is null) return NotFound();

            _context.Tags.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }

    }
}
