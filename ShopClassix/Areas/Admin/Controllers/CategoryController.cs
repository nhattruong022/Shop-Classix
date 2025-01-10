using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
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
        public IActionResult Edit(int id)
        {
            var category = _dataContext.categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost("Admin/Category/Edit/{id}")]
        public IActionResult Edit(int id, CategoryModel category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _dataContext.Entry(category).State = EntityState.Modified;
                _dataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet("Admin/Category/Add")]
        public IActionResult Add()
        {
            return View();
        }

        // POST: Admin/Category/Add
        [HttpPost("Admin/Category/Add")]
        public IActionResult Add(CategoryModel category) // Update method name
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dataContext.categories.Add(category); // Ensure this matches the DbSet
                    _dataContext.SaveChanges();

                    TempData["Message"] = "Category added successfully!";
                    return RedirectToAction("Index", "Category", new { area = "Admin" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cannot save data: " + ex.Message);
                }
            }
            return View(category); // Return the view with the invalid model
        }
    }
}
