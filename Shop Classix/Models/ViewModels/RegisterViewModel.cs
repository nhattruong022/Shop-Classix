using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models.ViewModels
{
    public class RegisterViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    public string Hoten { get; set; }

    [Key]
    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }  

    [Required(ErrorMessage = "Phone number is required.")]
    [MaxLength(10, ErrorMessage = "Phone number can have a maximum of 10 characters.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must have at least 8 characters.")]
    public string Password { get; set; }


        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; } = "User"; // Default role can be "User"


        //[Required(ErrorMessage = "Please choose your gender.")]
        //public bool? Gender { get; set; }

        //[Required(ErrorMessage = "Please choose your Date of Birth.")]
        //public DateTime? DateOfBirth { get; set; }

        //[Required(ErrorMessage = "Please enter your address.")]
        //public string Address { get; set; }

        // Nếu bạn cần thêm một hình ảnh, ví dụ:
        // [NotMapped] // Không lưu vào cơ sở dữ liệu
        // public IFormFile ImageUpload { get; set; }
    }


}
