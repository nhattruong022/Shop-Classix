namespace Shop_Classix.Models.ViewModels
{
    public class ProductSales
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public int QuantitySold { get; set; }
        public double TotalSales { get; set; }
        public double Revenue { get; set; }
        public double TotalProfit { get; set; }
        public double TotalCapital { get; set; }
    }
}
