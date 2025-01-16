using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Shop_Classix.Repository;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;

        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [Authorize(Roles="Admin")]
        [HttpGet("Admin/Home")]
        public IActionResult Index()
        {
            //hiển thị thông tin liên hệ lên dashboard
            var contactInfo = _dataContext.contacts.FirstOrDefault(); //lấy thông tin từ admin cntact
            return View(contactInfo);
        }
    }
}
