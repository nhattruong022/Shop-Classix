using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter your email.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must have at least 8 characters.")]
        public string password { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        public string Hoten { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public bool? gioitinh { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(60, ErrorMessage = "Address can have a maximum of 60 characters.")]
        public string? diachi { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(10, ErrorMessage = "Phone number can have a maximum of 10 characters.")]
        public string phoneNumber { get; set; }

        //public string? Hinh { get; set; }
    }

}
