using System.ComponentModel.DataAnnotations;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public IFormFile? HoverPhoto { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<int>? ImageIds { get; set; }
        public List<int>  TagsId{ get; set; }
        public List<int> ColorsId { get; set; }
        public List<int> SizesId { get; set; }
        public List<Category>?  Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Color>? Colors { get; set; }
        public List<Size>? Sizes { get; set; }

        public List<ProductImg>? ProductImages { get; set; }
        

    }
}
