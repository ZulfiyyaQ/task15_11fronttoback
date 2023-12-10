using Microsoft.AspNetCore.Identity;
using task15_11fronttoback.Utilities.Enums;

namespace task15_11fronttoback.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender gender { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<Order> Orders { get; set; }

    }
}
