using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Shop_Classix.Models;

namespace Shop_Classix.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<ProductsModel> products { get; set; }
        public DbSet<CategoryModel> categories { get; set; }
    }
}
