using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        [HttpGet("Admin/Product")]
        public IActionResult Index()
        {
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
