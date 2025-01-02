namespace Shop_Classix.Models.ViewModels
{
    public class CartItemViewModel
    {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice => Price * Quantity;
            public string ImageUrl { get; set; }
    }
}
