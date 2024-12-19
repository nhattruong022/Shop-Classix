using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Slug { get; set; } = string.Empty;

        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

           //cho phép CategoryModel lưu trữ nhiều đối tượng ProductModel.
        //Điều này có nghĩa là mỗi danh mục(category) có thể chứa nhiều sản phẩm.-> mối quan hệ một-nhiều
        public ICollection<ProductsModel> products { get; set; }

    }
}
