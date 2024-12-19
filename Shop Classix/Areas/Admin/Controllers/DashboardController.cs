using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _datacontext;

        public DashboardController(DataContext datacontext) {
            _datacontext = datacontext;
        }    


        [HttpGet("Admin/Dashboard/Chart")]
        public IActionResult Chart()
        {
            return View();
        }



        [HttpGet("Admin/Dashboard/Edit")]
        public async Task<IActionResult> Edit()
        {
            ContactModel contact = await _datacontext.contacts.FirstOrDefaultAsync();
            
            if(contact==null)
            {
                return NotFound();
            }    

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditContact(ContactModel contact)
        {
            var exist_contact=await _datacontext.contacts.FirstOrDefaultAsync();
            if(exist_contact==null)
            {
                ModelState.AddModelError("", "Contact is not exist");
                return View(contact);
            }

            if(ModelState.IsValid)
            {
                exist_contact.Logo=contact.Logo;
                exist_contact.map = contact.map;
                exist_contact.PhoneNumber = contact.PhoneNumber;
                exist_contact.Email = contact.Email;
                exist_contact.Address = contact.Address;

                await _datacontext.SaveChangesAsync();
                return RedirectToAction("Contact", "Dashboard");
            }
            return View(contact);
        }



        [HttpGet("Admin/Dashboard/Contact")]
        public IActionResult Contact()
        {
            var contacts = _datacontext.contacts.ToList();
            return View(contacts);
        }

 

        [HttpGet("Admin/Dashboard/Comments")]
        public IActionResult Comments()
        {
            return View();
        }
    }
}
