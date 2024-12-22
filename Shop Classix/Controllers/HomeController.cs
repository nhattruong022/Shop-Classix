using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using System.Diagnostics;

namespace Shop_Classix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext dataContext;
        public HomeController(ILogger<HomeController> logger,DataContext _datacontext)
        {
            dataContext=_datacontext;
            _logger = logger;
        }

        public IActionResult Index(int? categoryId)
        {
            var products = dataContext.products
                .Include(p => p.category)  // Lấy thông tin từ bảng Category
                .Where(p => !categoryId.HasValue || p.CategoryId == categoryId)  // Kiểm tra Id có khớp không
                .Select(p => new ProductsModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = Convert.ToDecimal(p.Price),
                    Image = p.Image,
                    Rating = p.Rating,
                    categoryName = p.category.Name // Lấy tên danh mục từ bảng Category
                })
                .ToList();

            return View(products);
        }
        


        public IActionResult DetailProduct()    
        {
            return View();
        }

        public IActionResult TimKiem()
        {
            return View();
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
