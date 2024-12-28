using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class AccountModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Please enter your password")]
        public string Password { get; set; } = string.Empty;    

        //mối quan hệ 1-n với customerModel
        //public virtual ICollection<CustomerModel> customers { get; set; }

        public int Type { get; set; }
        
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

        
    }
}
