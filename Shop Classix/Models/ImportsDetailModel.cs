using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Classix.Models
{
    [PrimaryKey(nameof(ImportId), nameof(ProductId))]
    public class ImportsDetailModel
    {
        public int ImportId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductCost { get; set; }

        // Tham chiếu đến Imports
        [ForeignKey("ImportId")]
        public virtual ImportsModel imports { get; set; }

        // Tham chiếu đến Products
        [ForeignKey("ProductId")]
        public virtual ProductsModel products { get; set; }
    }
}
