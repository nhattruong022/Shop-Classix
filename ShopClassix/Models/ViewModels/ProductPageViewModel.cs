namespace Shop_Classix.Models.ViewModels
{
    public class ProductPageViewModel
    {
        public IEnumerable<ProductsModel> AllProducts { get; set; } 
        public IEnumerable<ProductsModel> FavoriteProducts{ get; set; }
    }
}
