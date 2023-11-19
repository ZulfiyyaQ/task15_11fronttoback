using task15_11fronttoback.Models;

namespace task15_11fronttoback.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}
