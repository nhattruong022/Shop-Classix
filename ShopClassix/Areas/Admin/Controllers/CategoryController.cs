using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;

namespace Shop_Classix.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class CategoryController : Controller
    {
        public static string GenerateSlug(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            return name
                .ToLower()               // Chuyển tất cả ký tự thành chữ thường
                .Replace(" ", "-")       // Thay thế khoảng trắng bằng dấu gạch ngang
                .Replace(".", "")        // Xóa dấu chấm
                .Replace(",", "")        // Xóa dấu phẩy
                .Replace(":", "")        // Xóa dấu hai chấm
                .Replace(";", "")        // Xóa dấu chấm phẩy
                .Replace("?", "")        // Xóa dấu hỏi
                .Replace("!", "")        // Xóa dấu chấm than
                .Replace("&", "and")     // Thay dấu & bằng từ "and"
                .Replace("--", "-")      // Thay thế dấu gạch ngang kép thành một dấu gạch ngang
                .Trim('-');              // Xóa dấu gạch ngang dư thừa ở đầu hoặc cuối chuỗi
        }
        public readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("Admin/Category")]
        public IActionResult Index(string? name)
        {
            var categories = _dataContext.categories.ToList();
            if (!string.IsNullOrEmpty(name))
            {
                categories = categories.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (name != null && !categories.Any()) {
                ViewBag.categories = null;
                return View();
            }
            ViewBag.categories = categories;
            return View();
        }

        [HttpGet("Admin/Category/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            CategoryModel category = await _dataContext.categories.FindAsync(id);
            return View(category);
        }

        [HttpPost("Admin/Category/Edit/{id}")]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {

                category.Slug = GenerateSlug(category.Name);
                var existingSlug = await _dataContext.categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);

                if (existingSlug != null)
                {
                    ModelState.AddModelError("Slug", "Danh mục với tên này đã tồn tại.");
                    return View(category);
                }

                _dataContext.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }
        
        [HttpGet("Admin/Category/Add")]
        public async Task<IActionResult> Add()
        {
            ViewBag.Category = new SelectList(_dataContext.categories.ToList(), "Id", "Name");
            return View();
        }
        
        // POST: Admin/Category/Add
        [HttpPost]
        public async Task<IActionResult> Add(CategoryModel category) // Update method name
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if (ModelState.IsValid)
            {

                category.Slug = GenerateSlug(category.Name);
                var existingSlug = await _dataContext.categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);

                if (existingSlug != null)
                {
                    ModelState.AddModelError("Slug", "Danh mục với tên này đã tồn tại.");
                    return View(category);
                }

                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }    
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(category);

        }
    }
}
