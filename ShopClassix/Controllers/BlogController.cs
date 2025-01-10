using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;

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
