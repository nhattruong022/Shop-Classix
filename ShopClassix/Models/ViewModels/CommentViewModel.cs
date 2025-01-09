namespace Shop_Classix.Models.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; } 
        public string CustomerName { get; set; } = string.Empty; 
        public string ProductName { get; set; } = string.Empty; 
        public string ProductImage { get; set; } = string.Empty; 
        public string Category { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty; 
        public int Rating { get; set; } 
    }
}
