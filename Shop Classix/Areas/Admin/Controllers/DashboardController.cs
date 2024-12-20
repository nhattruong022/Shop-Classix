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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(DataContext datacontext,IWebHostEnvironment hostEnvironment) {
            _datacontext = datacontext;
            _webHostEnvironment = hostEnvironment;
        }    


        [HttpGet("Admin/Dashboard/Chart")]
        public IActionResult Chart()
        {
            return View();
        }

        [HttpGet("Admin/Dashboard/Contact")]
        public IActionResult Contact()
        {
            var contacts = _datacontext.contacts.ToList();
            return View(contacts);
        }


        //Edit contact
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

        //Edit contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(ContactModel contact)
        {
            var exist_contact=await _datacontext.contacts.FirstOrDefaultAsync();
            if(exist_contact==null)
            {
                ModelState.AddModelError("", "Contact is not exist");
                return View(contact);
            }


            if(ModelState.IsValid)
            {
                if(contact.ImageUpload!=null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Classix/img/logo");
                    
                    if(!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    String imageName=Guid.NewGuid().ToString()+"_"+Path.GetFileName(contact.ImageUpload.FileName);
                    string filePath=Path.Combine(uploadDir, imageName);

                    using (var fs=new FileStream(filePath,FileMode.Create))
                    {
                        await contact.ImageUpload.CopyToAsync(fs);
                    }
                    exist_contact.Logo = imageName;
                }

              
                exist_contact.map = contact.map;
                exist_contact.PhoneNumber = contact.PhoneNumber;
                exist_contact.Email = contact.Email;
                exist_contact.Address = contact.Address;

                await _datacontext.SaveChangesAsync();
                return RedirectToAction("Contact","Dashboard");
            }
            return View(contact);
        }





 

        [HttpGet("Admin/Dashboard/Comments")]
        public IActionResult Comments()
        {
            return View();
        }
    }
}
