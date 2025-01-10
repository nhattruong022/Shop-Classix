using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class ImportsModel
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }

		
		public double Cost { get; set; }
    }
}
