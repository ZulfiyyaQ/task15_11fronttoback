using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.Utilities.Extensions;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidesController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlidesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

            //bool result = _context.Slides.Any(s => s.Title.ToLower().Trim() == slide.Title.ToLower().Trim());
            //if (result)
            //{
            //    ModelState.AddModelError("Name", "Slide already exists");
            //    return View();
            //}

            if (slide.Photo is null)
            {
                ModelState.AddModelError("Photo", "File secilmesi mutleqdir");
                return View();
            }
            if (!slide.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Sekil file secmeyiniz mutleqdir");
                return View();
            }
            if (!slide.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "Sekil olcusu 2 mb dan artiq olmamalidir");
                return View();
            }


            slide.ImageUrl = await slide.Photo.CreateFile(_env.WebRootPath, "assets", "images","website-images");

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slide is null) return NotFound();

            return View(slide) ;
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, Slide slide)
        {
            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (slide.Photo is not null)
            {
                //bool result = _context.Slides.Any(s => s.Title== slide.Title && s.Id != id);
                //if (result)
                //{
                //    ModelState.AddModelError("Name", "Slide already exists");
                //    return View(existed);
                //}

                if (!slide.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "Sekil file secmeyiniz mutleqdir");
                    return View(existed);
                }
                if (!slide.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "Sekil olcusu 2 mb dan artiq olmamalidir");
                    return View(existed);
                }
                string newimage = await slide.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ImageUrl = newimage;
            }

            existed.Title = slide.Title;


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();

            slide.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

            _context.Slides.Remove(slide);
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