using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using task15_11fronttoback.Areas.Admin.ViewModels;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Include(x => x.Category).Include(p => p.ProductImages.Where(p => p.IsPrimary == true)).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductsVM productvm = new();
            GetList(ref productvm);
            return View(productvm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductsVM productvm)
        {

            if (!ModelState.IsValid)
            {
                GetList(ref productvm);
                return View();
            }


            bool result = await _context.Categories.AnyAsync(c => c.Id == productvm.CategoryId);
            if (!result)
            {
                GetList(ref productvm);
                ModelState.AddModelError("CategoryId", "Bele Id li category movcud deyil");
                return View();
            }

            foreach (int tagId in productvm.TagIds)
            {
                bool tagResult = await _context.Tags.AllAsync(t => t.Id == tagId);
                if (!result)
                {
                    GetList(ref productvm);
                    ModelState.AddModelError("TagIds", "Yalnis melumat daxil edilib");

                    return View();
                }
            }
            Product product = new Product
            {
                Name = productvm.Name,
                Price = productvm.Price,
                SKU = productvm.SKU,
                CategoryId = (int)productvm.CategoryId,
                Description = productvm.Description,
                ProductTags = new List<ProductTags>(),
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>()
            };

            foreach (int tagId in productvm.TagIds)
            {
                ProductTags productTag = new ProductTags
                {
                    TagId = tagId
                };
                product.ProductTags.Add(productTag);
            }
            foreach (int colorId in productvm.ColorIds)
            {
                ProductColor productcolor = new ProductColor
                {
                    ColorId = colorId
                };
                product.ProductColors.Add(productcolor);
            }
            foreach (int sizeId in productvm.SizeIds)
            {
                ProductSize productsize = new ProductSize
                {
                    SizeId = sizeId
                };
                product.ProductSizes.Add(productsize);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
  
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(pt => pt.ProductTags)
                .Include(pc=>pc.ProductColors)
                .Include(ps=>ps.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = existed.Name,
                Price = existed.Price,
                Description = existed.Description,
                SKU = existed.SKU,
                CategoryId = existed.CategoryId,
                Categories = await _context.Categories.ToListAsync(),
                TagsId = existed.ProductTags.Select(pt => pt.TagId).ToList(),
                Tags = await _context.Tags.ToListAsync(),
                ColorsId = existed.ProductColors.Select(pc => pc.ColorId).ToList(),
                Colors = await _context.Colors.ToListAsync(),
                SizesId = existed.ProductSizes.Select(pc => pc.SizeId).ToList(),
                Sizes = await _context.Sizes.ToListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors= await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                return View(productVM);
            }

            Product existed = await _context.Products.Include(p => p.ProductTags)
                .Include(pc => pc.ProductColors)
                .Include(ps => ps.ProductSizes)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();

            bool result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("Name", "Product already exists");
                return View();
            }


            bool result1 = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result1)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId", "Category not found, choose another one.");
                return View();
            }
            List<int> existedIds = new List<int>();
            //foreach (int tagId in productVM.TagsId)  { if (existed.ProductTags.Any(pt => pt.TagId == tagId)) { existedIds.Add(tagId); } }

            foreach (ProductTags protag in existed.ProductTags)
            {
                if (productVM.TagsId.Exists(tId => tId == protag.TagId)) { _context.ProductTags.Remove(protag); }
            }

            
            foreach (int tagId in productVM.TagsId)
            {
                if (!existed.ProductTags.Any(pt => pt.TagId == tagId))
                {
                    existed.ProductTags.Add(new ProductTags
                    {
                        TagId=tagId
                    });
                }
            }

            foreach (ProductColor procolor in existed.ProductColors)
            {
                if (productVM.ColorsId.Exists(tId => tId == procolor.ColorId)) { _context.ProductColors.Remove(procolor); }
            }


            foreach (int colorId in productVM.ColorsId)
            {
                if (!existed.ProductColors.Any(pt => pt.ColorId == colorId))
                {
                    existed.ProductColors.Add(new ProductColor
                    {
                        ColorId = colorId
                    });
                }
            }
            foreach (ProductSize prosize in existed.ProductSizes)
            {
                if (productVM.SizesId.Exists(cId => cId == prosize.SizeId)) { _context.ProductSizes.Remove(prosize); }
            }


            foreach (int sizeId in productVM.SizesId)
            {
                if (!existed.ProductSizes.Any(pt => pt.SizeId == sizeId))
                {
                    existed.ProductSizes.Add(new ProductSize
                    {
                        SizeId=sizeId
                    });
                }
            }

            existed.Name = productVM.Name;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.CategoryId = (int)productVM.CategoryId;
            existed.Description = productVM.Description;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductTags).ThenInclude(t => t.Tag)
                .Include(x => x.ProductColors).ThenInclude(c => c.Color)
                .Include(x => x.ProductSizes).ThenInclude(s => s.Size)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            return View(product);

        }
         public async Task<IActionResult> Delete (int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductTags).ThenInclude(t => t.Tag)
                .Include(x => x.ProductColors).ThenInclude(c => c.Color)
                .Include(x => x.ProductSizes).ThenInclude(s => s.Size)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound(); 

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private void GetList(ref CreateProductsVM vm)
        {
            vm.Categories = _context.Categories.ToList();
            vm.Tags = _context.Tags.ToList();
            vm.Colors = _context.Colors.ToList();
            vm.Sizes = _context.Sizes.ToList();
        }

    }
}
