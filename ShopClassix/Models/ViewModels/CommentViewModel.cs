namespace Shop_Classix.Models.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; } // Comment ID
        public string CustomerName { get; set; } = string.Empty; // Customer Name
        public string ProductName { get; set; } = string.Empty; // Product Name
        public string ProductImage { get; set; } = string.Empty; // Product Image
        public string Category { get; set; } = string.Empty; // Category
        public string Content { get; set; } = string.Empty; // Comment Content
        public int Rating { get; set; } // Rating (1 to 5)
    }
}
