using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using task15_11fronttoback.DAL;
using task15_11fronttoback.Interfaces;
using task15_11fronttoback.Models;
using task15_11fronttoback.Utilities.Exceptions;
using task15_11fronttoback.ViewModels;

namespace task15_11fronttoback.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager,IEmailService emailService )
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketvm = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
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
            if (id <= 0)  throw new WrongRequestException("Gonderilen sorgu yalnisdir") ;

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) throw new NotFoundException("Bele bir mehsul tapilmadi") ;

            if (User.Identity.IsAuthenticated)
            {
                //AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                AppUser user = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem item = user.BasketItems.FirstOrDefault(b => b.ProductId == id);
                if (item is null)
                {
                    item = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Price = product.Price,
                        Count = 1,
                        OrderId=null 
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
            if (id <= 0) throw new WrongRequestException("Gonderilen sorgu yalnisdir");

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) throw new NotFoundException("Bele bir mehsul tapilmadi");



            List<BasketCookieItemVM> basket = new List<BasketCookieItemVM>();

            if(User.Identity.IsAuthenticated)
            {
               

                AppUser user = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


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

        public async Task<IActionResult> CheckOut()
        {
            AppUser user=await _userManager.Users
                .Include(u=>u.BasketItems.Where(bi=>bi.OrderId==null))
                .ThenInclude(bi=>bi.Product)
                .FirstOrDefaultAsync(U=>U.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));

            OrderVM ordervm = new OrderVM
            { 
               BasketItems=user.BasketItems
            };

            return View(ordervm);
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut(OrderVM ordervm)
        {
            AppUser user = await _userManager.Users
               .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
               .ThenInclude(bi => bi.Product)
               .FirstOrDefaultAsync(U => U.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!ModelState.IsValid)
            {
                ordervm.BasketItems = user.BasketItems;
                return View(ordervm);
            }
            decimal total = 0;
            foreach (BasketItem item in user.BasketItems)
            {
                item.Price = item.Product.Price;
                total += item.Count * item.Price;
            }

            Order order = new Order
            { 
                Status=null,
                Address=ordervm.Address,
                PurchaseAt=DateTime.Now,
                AppUserId=user.Id,
                BasketItems=user.BasketItems,
                TotalPrice=total
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            string body = @"
    <html>
    <head>
        <style>
            table {
                border-collapse: separate;
                border-spacing: 0 10px;
                width: 80%; 
                margin: auto; 
            }
            th, td {
                border: 1px solid #dddddd;
                text-align: center; 
                padding: 10px;
            }
            th {
                background-color: #f2f2f2;
            }
            h2 {
                text-align: center; 
            }
            h4 {
                text-align: center;
                margin-bottom: 20px;
            }
        </style>
    </head>
    <body>
        <h2>Order Details</h2>
        <table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Price</th>
                    <th>Count</th>
                </tr>
            </thead>
            <tbody>";

foreach (var item in order.BasketItems)
{
    body += $@"
        <tr>
            <td>{item.Product.Name}</td>
            <td>{item.Price}</td>
            <td>{item.Count}</td>
        </tr>";
}

body += @"
            </tbody>
        </table>
    </body>
    </html>";

            await _emailService.SendMailAsync(user.Email, "Your Order", body, true);
            return RedirectToAction("Index", "Home");
        }

    }
}
