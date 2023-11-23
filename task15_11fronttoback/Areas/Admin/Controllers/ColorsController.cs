﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorsController : Controller
    {
        private readonly AppDbContext _context;

        public ColorsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Include(x=>x.ProductColors).ThenInclude(p=>p.Product).ToListAsync();
            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid) return View();

            bool result = _context.Colors.Any(c => c.Name.ToLower().Trim() == color.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bele reng artiq movcutdur");
                return View();
            }

            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Update(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

         Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Colors.Any(c => c.Name.ToLower().Trim() == color.Name.ToLower().Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele reng artiq movcutdur");
                return View();
            }
            existed.Name = color.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


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
