using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class ProductCommentModel
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
     
        public int Rating { get; set; }

        [ForeignKey("customers")]
        public int AccountId { get; set; }

        [ForeignKey("products")]
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        //tham chieu den Accounts
        public virtual CustomerModel customers { get; set; }
        //tham chieu den products
        public virtual ProductsModel products { get; set; }
    }
}
