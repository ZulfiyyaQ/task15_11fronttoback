using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Models;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
       

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
           
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
            return basketvm;
        }
    }
}
