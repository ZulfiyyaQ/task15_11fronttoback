using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using task15_11fronttoback.Areas.Admin.ViewModels;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.Utilities.Extensions;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]

    public class SlidesController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlidesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slidevm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            if (!slidevm.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Sekil file secmeyiniz mutleqdir");
                return View();
            }
            if (!slidevm.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "Sekil olcusu 2 mb dan artiq olmamalidir");
                return View();
            }

            
            string filename = await slidevm.Photo.CreateFile(_env.WebRootPath, "assets", "images","website-images");
            Slide slide = new Slide
            {
                ImageUrl = filename,
                Title = slidevm.Title,
                Subtitle = slidevm.Subtitle,
                Description = slidevm.Description,
                Order = slidevm.Order
            };

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slide is null) return NotFound();

            UpdateSlideVM slidevm = new UpdateSlideVM
            {
                ImageUrl = slide.ImageUrl,
                Title = slide.Title,
                Subtitle = slide.Subtitle,
                Description = slide.Description,
                Order = slide.Order

            };

            return View(slidevm) ;
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM slidevm)
        {
            

            if (!ModelState.IsValid)
            {
                return View();
            }

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            if (slidevm.Photo is not null)
            {
                //bool result = _context.Slides.Any(s => s.Title== slide.Title && s.Id != id);
                //if (result)
                //{
                //    ModelState.AddModelError("Name", "Slide already exists");
                //    return View(existed);
                //}

                if (!slidevm.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "Sekil file secmeyiniz mutleqdir");
                    return View(existed);
                }
                if (!slidevm.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "Sekil olcusu 2 mb dan artiq olmamalidir");
                    return View(existed);
                }
                string newimage = await slidevm.Photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ImageUrl = newimage;
            }

            existed.Title = slidevm.Title;


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
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