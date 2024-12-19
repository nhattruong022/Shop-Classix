using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Classix.Models
{
    public class ProductsModel
    {
        [Key]
        public int Id { get; set; }
             
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int FavoriteNumber { get; set; }
        public int Status { get; set; }

        
        public string Image { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        //[FileExtension]
        public IFormFile ImageUpLoad { get; set; }
    }
}
