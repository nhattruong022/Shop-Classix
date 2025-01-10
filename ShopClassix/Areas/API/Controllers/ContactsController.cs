using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using static QRCoder.PayloadGenerator.SwissQrCode;


namespace Shop_Classix.Areas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class ContactsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ContactsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet]
        public JsonResult GetFooterData()
        {
            var contacts = _dataContext.contacts
       .Select(c => new ContactModel
       {
           Name = c.Name,
          
           Email = c.Email,
           PhoneNumber = c.PhoneNumber,
           Address = c.Address,
           Logo = c.Logo
       })
         .FirstOrDefault();
            if (contacts == null)
            {
                return new JsonResult(new { message = "Không có dữ liệu" });
            }

            return new JsonResult(contacts); // Trả về dữ liệu dưới dạng JSON
        }
    }
}
