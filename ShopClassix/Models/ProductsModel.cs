using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shop_Classix.Repository.Validation;

namespace Shop_Classix.Models
{
        public class ProductsModel
        {
                [Key]
                public int Id { get; set; }

                [Required(ErrorMessage = "Please enter product name")]
                public string Name { get; set; } = string.Empty;

                [Required(ErrorMessage = "Please enter price")]
                public double Price { get; set; }
                public int Views { get; set; }

                [Required(ErrorMessage = "Plesae enter description")]
                public string Description { get; set; } = string.Empty;

                public int? FavoriteNumber { get; set; }

                public int Status { get; set; }

                [Required(ErrorMessage = "Please insert into image")]
                public string Image { get; set; } = string.Empty;

                public string? Slug { get; set; } = string.Empty;

                public double? Rating { get; set; }

                public string? Size { get; set; } = string.Empty;

                public int? Quantity { get; set; }
                public int? CategoryId { get; set; } //khóa ngoại
                public DateTime? CreatedAt { get; set; }
                public DateTime? UpdatedAt { get; set; }

                [NotMapped]
                [FileExtension]
                public IFormFile? ImageUpLoad { get; set; }

                public CategoryModel category { get; set; } //navigation property

                [NotMapped]
                public bool IsFavorite { get; set; }  //trạng thái yêu thích

                //lượt xem
                public int view { get; set; }

        }
}