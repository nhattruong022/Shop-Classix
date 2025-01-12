using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Shop_Classix.Models;
using Shop_Classix.Models.VnPay;

namespace Shop_Classix.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<ProductsModel> products { get; set; }
        public DbSet<FavoriteProductModel>favoriteProducts { get; set; }
        public DbSet<CategoryModel> categories { get; set; }
        public DbSet<CustomerModel> customers { get; set; }
        public DbSet<ContactModel> contacts { get; set; }   
        public DbSet<OrderModel> orders { get; set; }
        public DbSet<OrderDetailModel> orderDetails { get; set; }
        public DbSet<ChatUsersModel> chatUsers { get; set; }
        public DbSet<ImportsModel> imports { get; set; }
        public DbSet<ImportsDetailModel> importsDetails { get; set; }
        public DbSet<ProductCommentModel> productComments { get; set; }
        public DbSet<VnPayModel> vnPay { get; set; }
        

    }
}
