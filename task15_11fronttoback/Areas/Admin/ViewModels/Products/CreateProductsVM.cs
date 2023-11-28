using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IFormFile MainPhoto { get; set; }
        public IFormFile HoverPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
        public List<int>? ColorIds { get; set; }
        public List<int>? SizeIds { get; set; }
        public List< Tag>? Tags { get; set; }
        public List< Color>? Colors { get; set; }
        public List< Size>? Sizes { get; set; }
        public List<Category>? Categories { get; set; }





    }
}
