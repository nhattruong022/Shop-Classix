using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class AccountModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
