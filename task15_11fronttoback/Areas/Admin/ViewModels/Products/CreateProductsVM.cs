using System.ComponentModel.DataAnnotations;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.Areas.Admin.ViewModels
{
    public class CreateProductsVM
    {
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        [Required]
        public int? CategoryId { get; set; }
       
    }
}
