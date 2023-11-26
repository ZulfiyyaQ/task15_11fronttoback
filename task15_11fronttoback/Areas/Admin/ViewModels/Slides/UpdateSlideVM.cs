using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace task15_11fronttoback.Areas.Admin.ViewModels
{
    public class UpdateSlideVM
    {
        

        [Required(ErrorMessage = "Title daxil etmeyiniz mutleqdir")]
        [MaxLength(50, ErrorMessage = "Uzunlugu 50 den uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string Title { get; set; }

        [MaxLength(50, ErrorMessage = "Uzunlugu 50 den uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string Subtitle { get; set; }

        [MaxLength(200, ErrorMessage = "Uzunlugu 200 den uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Order daxil etmeyiniz mutleqdir")]
        public int Order { get; set; }
       
        public IFormFile? Photo { get; set; }
    }
}

