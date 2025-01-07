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

        [Column(TypeName = "decimal(18,2)")]
        public decimal deposit {  get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public int PaymentMethod { get; set; }



        [ForeignKey("CustomerId")]
        public virtual CustomerModel customers { get; set; } // Renamed from 'customers' to 'Customer'


        public string OrderNotes { get; set; }

        public int Status { get; set; }

        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        // One-to-many relationship with OrderDetail
        public ICollection<OrderDetailModel> orderDetails { get; set; }

        
    }

}


