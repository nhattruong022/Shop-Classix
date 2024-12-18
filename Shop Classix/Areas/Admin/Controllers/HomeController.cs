using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        [HttpGet("Admin/Home")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
