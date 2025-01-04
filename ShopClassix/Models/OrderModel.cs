using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public int PaymentMethod { get; set; }

        public int CustomerId { get; set; }


		[ForeignKey("CustomerId")]
        public CustomerModel customers { get; set; }

		public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
