using System.ComponentModel.DataAnnotations;
using task15_11fronttoback.Utilities.Enums;

namespace task15_11fronttoback.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MaxLength(30, ErrorMessage = "Uzunlugu 30 dan uzun olmamalidir")]
        [MinLength(4, ErrorMessage = "Uzunlugu en azi 4 olmalidir")]
        public string UserName { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Uzunlugu 30 dan uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string Name { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Uzunlugu 30 dan uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string Surname { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(256, ErrorMessage = "Uzunlugu 256 dan uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public Gender Genders { get; set; }

    }
}
