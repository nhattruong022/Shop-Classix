using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("Admin/Category/Edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet("Admin/Category/Add")]
        public IActionResult Add()
        {
            return View();
        }
    }
}
