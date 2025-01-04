using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Classix.Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class OrderDetailModel
    {
        [Key]
        public int OrderId { get; set; }

        [Key]        
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Tham chiếu đến Orders
        [ForeignKey("OrderId")]
        public virtual OrderModel Orders { get; set; }

        // Tham chiếu đến Products
        [ForeignKey("ProductId")]
        public virtual ProductsModel Products { get; set; }
    }
}
