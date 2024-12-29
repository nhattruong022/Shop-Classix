using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;
using Shop_Classix.Service;

namespace Shop_Classix.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailService _emailService;

        public ContactController(DataContext dataContext, IEmailService emailService)
        {
            _dataContext = dataContext;
            _emailService = emailService;
        }


        [HttpPost]
        public async Task<IActionResult>SendContactEmail(string firstName,string lastName,string email,string message)
        {
            //Nội dung email
            //$@ cho phép không cần viết /n để xuống dòng và khi cộng các biến thì không cần có "" ->{firstname}+""+"{lastname} 
            string subject = "Liên hệ mới từ khách hàng";
            string body = $@"
                <h4>Thông tin khách hàng: </h4>
                <p><strong>Họ và tên: </strong>{firstName}  {lastName}</p>
                <p><strong>Email:</strong>{email}</p>
                <p><strong>Lời nhắn: </strong>{message}</p>";


            //gửi email
            await _emailService.SendEmail("nhattruongp78@gmail.com",subject,body);

            return RedirectToAction("Contact");
        }

        public IActionResult Contact()
<<<<<<< HEAD
        {
			var connectshop = _dataContext.contacts.ToList();
			ViewBag.Connectshop = connectshop;
			return View();
		}
=======
        {
            var connectshop = _dataContext.contacts.ToList();
            ViewBag.Connectshop = connectshop;
            return View();
        }
>>>>>>> f42b1965892b1ad10d049ab12c14fa76acf00cb0
    }
}
