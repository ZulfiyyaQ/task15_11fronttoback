﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Controllers
{
    public class ProductsController1 : Controller
    {
        private readonly AppDbContext _context;


        public ProductsController1(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            Product product = _context.Products.Include(x => x.Category).Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }


            ProductVM productvm = new ProductVM
            {
                Product = product,
                RelatedProducts = _context.Products.Where(p => p.Category.Id == product.CategoryId && p.Id != product.Id).Include(x => x.ProductImages).ToList(),
            };


           
            return View(productvm);
        }



       
    }
}
