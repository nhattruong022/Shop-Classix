namespace Shop_Classix.Models
{
    public class ProductPost
    {
        public int Id { get; set; } // ID bài viết
        public string Title { get; set; } // Tiêu đề bài viết
        public string Content { get; set; } // Nội dung bài viết
        public string Keywords { get; set; } // Từ khóa để tìm kiếm (có thể là các sản phẩm hoặc danh mục)
        public DateTime PublishedDate { get; set; } // Ngày đăng
    }
}
