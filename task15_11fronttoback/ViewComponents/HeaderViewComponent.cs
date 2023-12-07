using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public HeaderViewComponent(AppDbContext context,UserManager<AppUser> userManager,IHttpContextAccessor http)
        {
            _context = context;
            _userManager = userManager;
            _http = http;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            List<BasketItemVM> basketvm = await CardAsync();

            
            var viewModel = new HeaderVM
            {
                Settings = settings,
                BasketItems = basketvm
            };

            return View(viewModel);
            
        }

        public async Task<List<BasketItemVM>> CardAsync()
        {
            List<BasketItemVM> basketvm = new List<BasketItemVM>();


            if(_http.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users.Include(u => u.BasketItems)
                   .ThenInclude(bi => bi.Product)
                   .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id ==_http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem item in user.BasketItems)
                {
                    basketvm.Add(new BasketItemVM
                    {  
                        Id=item.ProductId,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        Count = item.Count,
                        SubTotal = item.Product.Price * item.Count,
                        Image = item.Product.ProductImages.FirstOrDefault()?.Url

                    });

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




           
            return basketvm;
        }
    }
}
