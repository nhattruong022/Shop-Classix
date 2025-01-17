using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop_Classix.Controllers
{

    public class AboutController : Controller
    {
        public IActionResult About()
        {

            return View();
        }
    }
}
