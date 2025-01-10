using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;


namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private const int PageSize = 5;
        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet("Admin/Product")]
        public async Task<IActionResult> Index(string? name, string cate, int page = 1)
        {
            var productQuery = _dataContext.products.Include(p => p.category).AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(name))
            {
                
                productQuery = productQuery.Where(p => p.Name.Contains(name));
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(cate))
            {
                productQuery = productQuery.Where(p => p.category.Name == cate);
            }
            if (name != null && !productQuery.Any())
            {
                ViewBag.Products = null;
                return View();
            }
            // Tính tổng số sản phẩm sau khi áp dụng bộ lọc
            var totalOrders = await productQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

            // Lấy danh sách sản phẩm với phân trang
            var products = await productQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Gán dữ liệu cho ViewBag
            ViewBag.Products = products;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dataContext.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Status--;
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Active(int id)
        {
            var product = await _dataContext.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Status++;
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }

}
