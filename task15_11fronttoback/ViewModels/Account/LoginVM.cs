using System.ComponentModel.DataAnnotations;

namespace task15_11fronttoback.ViewModels
{
    public class LoginVM
    {
        [Required]
        [MaxLength(256, ErrorMessage = "Uzunlugu 256 dan uzun olmamalidir")]
        [MinLength(3, ErrorMessage = "Uzunlugu en azi 3 olmalidir")]
        public string UsernameOrEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool IsRemembered { get; set; }
    }
}
