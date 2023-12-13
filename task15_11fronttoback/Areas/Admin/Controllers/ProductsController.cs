using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using task15_11fronttoback.Areas.Admin.ViewModels;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.Utilities.Extensions;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]

    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index(int page = 1)
        {
            double count = await _context.Products.CountAsync();
           
            List<Product> products = await _context.Products.Skip((page - 1) * 3).Take(3)
                .Include(x => x.Category)
                .Include(p => p.ProductImages.Where(p => p.IsPrimary == true))
                .ToListAsync();

            PaginationVM<Product> paginateVM = new PaginationVM<Product>
            {
                CurrentPage = page,
                TotalPage = Math.Ceiling(count / 3),
                Items = products

            };
            return View(paginateVM);
        }
        [Authorize(Roles = "Admin,Moderator")]
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

            if (!productvm.MainPhoto.ValidateType("image/"))
            {
                GetList(ref productvm);
                ModelState.AddModelError("MainPhoto", "File tipi uygun deyil ");
                return View();
            }

            if (!productvm.MainPhoto.ValidateSize(600))
            {
                GetList(ref productvm);
                ModelState.AddModelError("MainPhoto", "File olcusu uygun deyil");
                return View();
            }

            if (!productvm.HoverPhoto.ValidateType("image/"))
            {
                GetList(ref productvm);
                ModelState.AddModelError("HoverPhoto", "File tipi uygun deyil ");
                return View();
            }

            if (!productvm.HoverPhoto.ValidateSize(600))
            {
                GetList(ref productvm);
                ModelState.AddModelError("HoverPhoto", "File olcusu uygun deyil");
                return View();
            }

            ProductImg img = new ProductImg
            {
                IsPrimary = true,
                Url = await productvm.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
            };
            ProductImg hoverimg = new ProductImg
            {
                IsPrimary = false,
                Url = await productvm.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
            };


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
                ProductSizes = new List<ProductSize>(),
                ProductImages = new List<ProductImg> { img, hoverimg }

            };
            TempData["Message"] = "";
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


            foreach (var photo in productvm.Photos)
            {
                if (!photo.ValidateType("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil  </p>";
                    continue;
                }
                if (!photo.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil </p> ";
                    continue;
                }

                product.ProductImages.Add(new ProductImg
                {
                    IsPrimary = null,
                    Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
                });
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p => p.ProductImages).Include(pt => pt.ProductTags)
                .Include(pc => pc.ProductColors)
                .Include(ps => ps.ProductSizes)
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
                Sizes = await _context.Sizes.ToListAsync(),
                ProductImages = existed.ProductImages
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products.Include(pi => pi.ProductImages).Include(p => p.ProductTags)
                .Include(pc => pc.ProductColors)
                .Include(ps => ps.ProductSizes)
                .FirstOrDefaultAsync(e => e.Id == id);
            productVM.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                GetList(ref productVM);
                return View(productVM);
            }


            if (existed is null) return NotFound();


            bool result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                GetList(ref productVM);
                ModelState.AddModelError("Name", "Product already exists");
                return View(productVM);
            }


            bool result1 = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result1)
            {
                GetList(ref productVM);
                ModelState.AddModelError("CategoryId", "Category not found, choose another one.");
                return View(productVM);
            }

            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    GetList(ref productVM);
                    ModelState.AddModelError("MainPhoto", "Tipi uygun deyil");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(600))
                {
                    GetList(ref productVM);
                    ModelState.AddModelError("MainPhoto", "Olcusu uygun deyil");
                    return View(productVM);
                }

            }
            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.ValidateType("image/"))
                {
                    GetList(ref productVM);
                    ModelState.AddModelError("HoverPhoto", "Tipi uygun deyil");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.ValidateSize(600))
                {
                    GetList(ref productVM);
                    ModelState.AddModelError("HoverPhoto", "Olcusu uygun deyil");
                    return View(productVM);
                }

            }
            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");

                ProductImg mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                _context.ProductImages.Remove(mainImage);

                existed.ProductImages.Add(new ProductImg
                {
                    IsPrimary = true,
                    Url = fileName
                });
            }
            if (productVM.HoverPhoto is not null)
            {
                string fileName = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "images", "website-images");

                ProductImg hoverImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                hoverImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                _context.ProductImages.Remove(hoverImage);

                existed.ProductImages.Add(new ProductImg
                {
                    IsPrimary = false,
                    Url = fileName
                });
            }


            List<int> existedIds = new List<int>();

            //List<ProductTags> removeable = existed.ProductTags.Where(pt => !productVM.TagsId.Exists(tId => tId == pt.TagId)).ToList();
            //_context.ProductTags.RemoveRange(removeable);

            //foreach (ProductTags protag in existed.ProductTags)
            //{
            //    if (productVM.TagsId.Exists(tId => tId == protag.TagId)) { _context.ProductTags.Remove(protag); }
            //}

            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }

            List<ProductImg> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImg pImage in removeable)
            {
                pImage.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(pImage);
            }


            existed.ProductTags.RemoveAll(pt => !productVM.TagsId.Exists(tId => tId == pt.TagId));


            List<int> creatable = productVM.TagsId.Where(tId => !existed.ProductTags.Exists(pt => pt.TagId == tId)).ToList();
            foreach (int tagId in creatable)
            {
                bool tagResult = await _context.Tags.AnyAsync(t => t.Id == tagId);
                if (!tagResult)
                {
                    GetList(ref productVM);
                    ModelState.AddModelError("TagIds", "Tag not found, choose another one.");
                    return View();

                }
                existed.ProductTags.Add(new ProductTags
                {
                    TagId = tagId
                });
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
                        SizeId = sizeId
                    });
                }
            }

            TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (var photo in productVM.Photos)
                {
                    if (!photo.ValidateType("image/"))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil  </p>";
                        continue;
                    }
                    if (!photo.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil </p> ";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImg
                    {
                        IsPrimary = null,
                        Url = await photo.CreateFile(_env.WebRootPath, "assets", "images", "website-images")
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
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

            foreach (ProductImg image in product.ProductImages)
            {
                image.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            }

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
        private void GetList(ref UpdateProductVM vm)
        {
            vm.Categories = _context.Categories.ToList();
            vm.Tags = _context.Tags.ToList();
            vm.Colors = _context.Colors.ToList();
            vm.Sizes = _context.Sizes.ToList();
        }

    }
}
