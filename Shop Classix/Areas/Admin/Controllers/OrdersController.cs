using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        [HttpGet("Admin/Orders")]
        public IActionResult Orders()
        {
            return View();
        }
    }
}
