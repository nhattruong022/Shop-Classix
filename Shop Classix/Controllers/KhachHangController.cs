using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Shop_Classix.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly DataContext _dataContext;

        public KhachHangController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

			[HttpPost]
			public IActionResult Register(RegisterViewModel model)
			{
				if (ModelState.IsValid)
				{
					//Kiểm tra email đã tồn tại
					var existingUser = _dataContext.customers.FirstOrDefault(c => c.Email == model.Email);

					if (existingUser != null)
					{
						ModelState.AddModelError("Email", "Email already exists.");
						return View(model);
					}

					// Mã hóa mật khẩu
					var passwordHasher = new PasswordHasher<CustomerModel>();
					var hashedPassword = passwordHasher.HashPassword(new CustomerModel(), model.Password);

					// Tạo khách hàng mới
					var newCustomer = new CustomerModel
					{
						Name = model.Hoten,
						Email = model.Email,
						PhoneNumber = model.PhoneNumber,
						Password = hashedPassword
						//Gender = model.Gender,
						//DateOfBirth = model.DateOfBirth,
						//Address = model.Address
					};

					// Thêm vào cơ sở dữ liệu
					_dataContext.customers.Add(newCustomer);
					_dataContext.SaveChanges();

					// Chuyển hướng đến trang đăng nhập
					return RedirectToAction("Login", "KhachHang");
				}

				// Nếu ModelState không hợp lệ
				return View(model);
			}



		[HttpGet]
		public IActionResult Login(string returnUrl)
		{
		    ViewBag.returnUrl = returnUrl;

		    return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
		{
		    if (ModelState.IsValid)
		    {
		        var khachHang = _dataContext.customers.SingleOrDefault(kh => kh.Email == model.Email);

		        if (khachHang == null)
		        {
		            ModelState.AddModelError("loi", "Không có khách hàng này");
		            return View();
		        }
		        else if (khachHang.Password !=model.Password)
		        {
		            ModelState.AddModelError("loi", "Mật khẩu không đúng");
		            return View();
		       }
		       else
		       {
		           var claims = new List<Claim>
		   {
		       new Claim(ClaimTypes.Email, khachHang.Email),
		       new Claim(ClaimTypes.NameIdentifier, khachHang.Id.ToString())
		   };

		           var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		           var authProperties = new AuthenticationProperties
		           {
		               IsPersistent = true,
		               RedirectUri = returnUrl ?? Url.Action("Index", "Home")
		           };

		           await HttpContext.SignInAsync(
		               CookieAuthenticationDefaults.AuthenticationScheme,
		               new ClaimsPrincipal(claimsIdentity),
		               authProperties);

		           return Redirect(returnUrl ?? Url.Action("Index", "Home"));
		       }
		   }

		   return View();
		}



		public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }


        public IActionResult MyOrder()
        {
            return View();  
        }

        public IActionResult Comments()
        {
            return View();
        }

    

    }
}
