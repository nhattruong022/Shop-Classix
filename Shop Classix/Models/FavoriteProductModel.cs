using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Shop_Classix.Models
{
    [PrimaryKey(nameof(CustomerId),nameof(ProductId))]
    public class FavoriteProductModel
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

       //tham chieu den Customers
        [ForeignKey(nameof(CustomerId))]
        public virtual CustomerModel customers { get; set; }

        //tham chieu den Products
        [ForeignKey(nameof(ProductId))]
        public virtual ProductsModel products { get; set; }
    }
}
