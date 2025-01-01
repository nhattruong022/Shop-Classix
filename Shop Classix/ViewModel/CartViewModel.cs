namespace Shop_Classix.ViewModel
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
         
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
    }
}
