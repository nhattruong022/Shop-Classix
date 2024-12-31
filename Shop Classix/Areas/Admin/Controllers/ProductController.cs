using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;


namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet("Admin/Product")]
        public IActionResult Index(string? name, string cate)
        {
            var products = _dataContext.products.Include(p => p.category).AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(name))
            {
                products = products
                    .Where(p => p.Name.Contains(name));
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(cate))
            {
                products = products
                    .Where(p => p.category.Name == cate);
            }

            // Chuyển đổi kết quả thành danh sách và gán cho ViewBag
            ViewBag.Products = products.ToList(); // Chuyển đổi thành danh sách để thực thi truy vấn

            return View();
        }
        [HttpGet("Admin/Product/Edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet("Admin/Product/Add")]
        public IActionResult Add()
        {
            return View();
        }
    }
}
