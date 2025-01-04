using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Sign_in()
        {
            return View();
        }

        public IActionResult Sign_up()
        {
            return View();
        }
    }
}
