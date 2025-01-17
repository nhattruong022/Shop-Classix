using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Helper;
using Shop_Classix.Repository;


namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private const int PageSize = 5;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
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

        // GET: Admin/Product/Add
        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(await _dataContext.categories.ToListAsync(), "Id", "Name");
            return View();  
        }

        // POST: Admin/Product/Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add(ProductsModel product)
        {
            // Kiểm tra Slug trùng lặp
            product.Slug = Slug.GenerateSlug(product.Name);
            if (await _dataContext.products.AnyAsync(p => p.Slug == product.Slug))
            {
                ModelState.AddModelError("Slug", "Sản phẩm với tên này đã tồn tại.");
                ViewBag.Categories = new SelectList(await _dataContext.categories.ToListAsync(), "Id", "Name");
                return View(product);
            }

            // Xử lý upload file ảnh
            if (product.ImageUpLoad != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string imageName = Guid.NewGuid() + "_" + product.ImageUpLoad.FileName;
                string filePath = Path.Combine(uploadDir, imageName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageUpLoad.CopyToAsync(fs);
                }
                product.Image = imageName;
            }

            // Gán các giá trị mặc định
            product.Views = 100;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            // Lưu sản phẩm vào database
            await _dataContext.AddAsync(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("Admin/Product/Edit")]
        public IActionResult Edit()
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
        public async Task<IActionResult> Activate(int id)
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
