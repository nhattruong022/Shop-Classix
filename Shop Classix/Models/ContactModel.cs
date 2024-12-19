using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Classix.Models
{
    public class ContactModel
    {
        [Key]

        [Required(ErrorMessage = "yeu cau nhap ten danh muc")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="Yêu cầu nhập bản đồ")]
        public string map {  get; set; }

        [Required(ErrorMessage = "yeu cau nhap ten danh muc")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "yeu cau nhap ten danh muc")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "yeu cau nhap ten danh muc")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "yeu cau nhap ten danh muc")]
        public string Logo { get; set; } = string.Empty;


        [NotMapped]
      //[FileExtension]
        public IFormFile ImageUpload {  get; set; }
    }
}
