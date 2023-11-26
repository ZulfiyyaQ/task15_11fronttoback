using System.ComponentModel.DataAnnotations;

namespace task15_11fronttoback.Areas.Admin.ViewModels
{
    public class CreateSizesVM
    {
        [Required(ErrorMessage = "Ad daxil etmeyiniz mutleqdir")]
        [MaxLength(30, ErrorMessage = "Uzunlugu 30 dan uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string Name { get; set; }
    }
}
