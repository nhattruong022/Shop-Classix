using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shop_Classix.Repository.Validation;
using Microsoft.EntityFrameworkCore;


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

        [Required(ErrorMessage = "Please enter your phone")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; } = string.Empty;

        public string? Address { get; set; } = string.Empty;



        public bool? Gender { get; set; }


        public DateTime? DateOfBirth { get; set; }

        public string? Role { get; set; } = "User";



        public string? image {  get; set; } = string.Empty;



   
    }
}
