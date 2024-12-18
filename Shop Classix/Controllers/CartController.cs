using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }
        
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
