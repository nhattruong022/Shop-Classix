using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool Gender { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string image {  get; set; } = string.Empty;

        [ForeignKey("accounts")]
        public int AccountId { get; set; }

        //tham chieu den Accounts
        public virtual AccountModel accounts { get; set; }

        [NotMapped]
      //[FileExtension]
        public IFormFile ImageUpload { get; set; }
    }
}
