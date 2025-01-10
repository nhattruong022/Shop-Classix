namespace Shop_Classix.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        public double TotalAmount => Items.Sum(item => item.TotalPrice);
    }
}
