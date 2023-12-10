using task15_11fronttoback.Models;

namespace task15_11fronttoback.ViewModels
{
    public class OrderVM
    {
        public string Address { get; set; }
        public List<BasketItem>? BasketItems{ get; set; }
    }
}
