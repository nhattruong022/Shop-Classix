using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        [HttpGet("Admin/Category")]
        public IActionResult Index()
        {
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
