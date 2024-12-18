using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
    }
}
