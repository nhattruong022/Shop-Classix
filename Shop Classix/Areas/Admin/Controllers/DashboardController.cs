using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        [HttpGet("Admin/Dashboard/Chart")]
        public IActionResult Chart()
        {
            return View();
        }

        [HttpGet("Admin/Dashboard/Edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet("Admin/Dashboard/Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet("Admin/Dashboard/Comments")]
        public IActionResult Comments()
        {
            return View();
        }
    }
}
