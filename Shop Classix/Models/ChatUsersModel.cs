using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class ChatUsersModel
    {
        [Key]
        public int Id { get; set; }
        public string? MessageContent { get; set; }
        [ForeignKey("accounts")]
        public int AccountId { get; set; }
        public DateTime CreateAt { get; set; }
        //tham chieu den Accounts
        public virtual AccountModel accounts { get; set; }
    }
}
