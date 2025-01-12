using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models.VnPay
{
    public class VnPayModel
    {
        [Key]
        public int Id {  get; set; }
        public string? OrderId { get; set; }
        public string? description { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionId {  get; set; }
        public string? PaymentId { get; set; }
        public DateTime? createdAt { get; set;}



    }
}
