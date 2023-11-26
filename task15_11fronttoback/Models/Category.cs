using System.ComponentModel.DataAnnotations;

namespace task15_11fronttoback.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad daxil etmeyiniz mutleqdir")]
        [MaxLength(30, ErrorMessage = "Uzunlugu 30 dan uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]

        public string Name { get; set; }

        public List<Product>? Products { get; set; }
    }
}
