using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;


namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidesController : Controller
    {

        private readonly AppDbContext _context;

        public SlidesController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            

            bool result = _context.Slides.Any(s => s.Order < 0);
            if (result)
            {
                ModelState.AddModelError("Order", "Order 0 dan asagi ola bilmez");
                return View();
            }
            if (!slide.Photo.ContentType.Contains("image"))
            {
                ModelState.AddModelError("Photo", "File Secmeyiniz mutleqdir");
                return View();
            }
            if (slide.Photo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Photo", "Sekil 2 mb dan cox ola bilmez");
                return View();
            }

            string currentdirectory = Directory.GetCurrentDirectory();

            using (FileStream file = new FileStream(@$"{currentdirectory}/wwwroot/assets/images/slider/{slide.Photo.FileName}", FileMode.Create))
            {
                await slide.Photo.CopyToAsync(file);
            }

            slide.ImageUrl = slide.Photo.FileName;
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            return View(slide);
        }



    }
} 