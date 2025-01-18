namespace Shop_Classix.Models
{
    public class BlogPost
    {
        public int Id { get; set; } // ID bài viết
        public string Title { get; set; } // Tiêu đề bài viết
        public string Content { get; set; } // Nội dung bài viết
        public DateTime PublishedDate { get; set; } // Ngày đăng
    }
}
