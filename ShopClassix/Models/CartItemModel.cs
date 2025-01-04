using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Classix.Models
{
    [PrimaryKey(nameof(CustomerId), nameof(ProductId))]
    public class CartItemModel
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }

        // Tham chiếu đến Customer
        [ForeignKey("CustomerId")]
        public virtual CustomerModel customers { get; set; }

        // Tham chiếu đến Product
        [ForeignKey("ProductId")]
        public virtual ProductsModel products { get; set; }
    }
}
