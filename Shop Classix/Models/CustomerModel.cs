using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shop_Classix.Repository.Validation;


namespace Shop_Classix.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter your name")]  
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Please enter your password")]
        public string Password { get; set; }= string.Empty;

        [Required(ErrorMessage = "Please enter your phone")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your address")]
        public string? Address { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please choose your gender")]
        public bool? Gender { get; set; }

        [Required(ErrorMessage = "Please choose your DateOfBirt h")]
        public DateTime? DateOfBirth { get; set; }

        //public string? image {  get; set; } = string.Empty;

        [ForeignKey("accounts")]
        public int AccountId { get; set; }

        //tham chieu den Accounts
        public virtual AccountModel accounts { get; set; }

        //[NotMapped]
        //[FileExtension]
        //public IFormFile ImageUpload { get; set; }
    }
}
