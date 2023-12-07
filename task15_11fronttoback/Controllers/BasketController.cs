using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketvm = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users.Include(u => u.BasketItems)
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem item in user.BasketItems)
                {
                    basketvm.Add(new BasketItemVM
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        Count = item.Count,
                        SubTotal = item.Product.Price * item.Count,
                        Image=item.Product.ProductImages.FirstOrDefault()?.Url

                    }) ;

                }
            }
            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    foreach (var basketcookieitem in basket)
                    {
                        Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == basketcookieitem.Id);
                        if (product is not null)
                        {
                            BasketItemVM basketItemVm = new BasketItemVM
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Image = product.ProductImages.FirstOrDefault().Url,
                                Price = product.Price,
                                Count = basketcookieitem.Count,
                                SubTotal = product.Price * basketcookieitem.Count

                            };
                            basketvm.Add(basketItemVm);
                        }
                    }
                }
            }


            return View(basketvm);
        }
        public async Task<IActionResult> Addbasket(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                //AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                AppUser user = await _userManager.Users.Include(u => u.BasketItems).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem item = user.BasketItems.FirstOrDefault(b => b.ProductId == id);
                if (item is null)
                {
                    item = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Price = product.Price,
                        Count = 1
                    };
                    //await _context.BasketItems.AddAsync(item);
                    user.BasketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }

                if (user is null) return NotFound();


                await _context.SaveChangesAsync();

            }
            else
            {
                List<BasketCookieItemVM> basket;

                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    BasketCookieItemVM item = basket.FirstOrDefault(b => b.Id == id);

                    if (item is null)
                    {
                        BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                        {
                            Id = id,
                            Count = 1
                        };

                        basket.Add(basketCookieItemVM);
                    }
                    else
                    {
                        item.Count++;
                    }
                }
                else
                {
                    basket = new List<BasketCookieItemVM>();
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };

                    basket.Add(basketCookieItemVM);

                }


                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("Basket", json);
            }



            return RedirectToAction(nameof(Index), "Home");
        }
       
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

           

            List<BasketCookieItemVM> basket = new List<BasketCookieItemVM>();

            if(User.Identity.IsAuthenticated)
            {
               

                AppUser user = await _userManager.Users.Include(u => u.BasketItems).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                if (user is not null)
                {
                    
                    BasketItem basketItem = user.BasketItems.FirstOrDefault(item => item.ProductId == id);

                    
                    if (basketItem is not null)
                    {
                        
                        _context.BasketItems.Remove(basketItem);
                        await _context.SaveChangesAsync();
                    }
                }

            }
            else
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                    BasketCookieItemVM basketCookieItemVM = basket.FirstOrDefault(item => item.Id == id);

                    basket.Remove(basketCookieItemVM);

                }


                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("Basket", json);

            }


            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> GetBasket()
        {
            return Content(Request.Cookies["Basket"]);
        }
    }
}
