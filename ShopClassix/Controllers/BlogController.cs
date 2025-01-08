using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Blog()
        {
            return View();
        }
    }
}
