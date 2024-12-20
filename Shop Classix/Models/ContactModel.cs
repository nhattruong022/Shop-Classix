using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shop_Classix.Repository.Validation;

namespace Shop_Classix.Models
{
    public class ContactModel
    {
        [Key]

        [Required(ErrorMessage = "yeu cau nhap ten danh muc")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="Yêu cầu nhập bản đồ")]
        public string map {  get; set; }

        [Required(ErrorMessage = "yeu cau nhap Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "yeu cau nhap số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "yeu cau nhap địa chỉ")]
        public string Address { get; set; } = string.Empty;

        public string Logo { get; set; } = string.Empty;


        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload {  get; set; }
    }
}
