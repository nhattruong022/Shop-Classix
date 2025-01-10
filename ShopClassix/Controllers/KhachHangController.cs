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
				var existingUser = _dataContext.customers.FirstOrDefault(c => c.Email != null && c.Email == model.Email);

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
					Password = hashedPassword,
					Role = model.Role ?? "User"
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
		public IActionResult Login(LoginViewModel model, string? returnUrl)
		{
			if (ModelState.IsValid)
			{
				//kiểm tra email có tồn tại trong database ko

				var khachHang = _dataContext.customers.SingleOrDefault(kh => kh.Email == model.Email);

				if (khachHang == null)
				{
					ModelState.AddModelError("loi", "This customer does not exist");
					return View();
				}

				// Kiểm tra mật khẩu đã mã hóa
				var passwordHasher = new PasswordHasher<CustomerModel>();
				var passwordVerificationResult = passwordHasher.VerifyHashedPassword(khachHang, khachHang.Password, model.Password);

				if (passwordVerificationResult == PasswordVerificationResult.Failed)
				{
					ModelState.AddModelError("loi", "Password is not correct");
					return View();
				}
				else
				{
                   
                    //Nếu người dùng nhập đúng thông tin: Tạo ra các claim
                    //thiết lập cookie xác thực

                    var claims = new List<Claim>
				   {
					   new Claim(ClaimTypes.Email, khachHang.Email), //email khách hàng
					   new Claim(ClaimTypes.NameIdentifier, khachHang.Id.ToString()), //id khách hàng
					   new Claim(ClaimTypes.Name,khachHang.Email), //gán email vào claimtypes.name để hiển thị trong @User.identity.Name
					   new Claim(ClaimTypes.Role,khachHang.Role??"User")
				   };

					//ClaimsIdentity: Đại diện cho danh tính của người dùng. Nó chứa danh sách các "Claim" và thông tin về cách xác thực
					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


					//cấu hình thuộc tính xác thực
					var authProperties = new AuthenticationProperties
					{
						IsPersistent = true,  //cookie tồn tại khi người dùng đóng trình duyệt
						RedirectUri = returnUrl ?? Url.Action("Index", "Home")   //nếu không có ReturnUrl, chuyển đến trang chủ
					};

					HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties).Wait();




					if (khachHang.Role == "Admin")
					{
						return RedirectToAction("Index", "Home", new { area = "Admin" });  // Redirect to Admin area, Admin/Index
					}
					else
					{
						return Redirect(returnUrl ?? Url.Action("Index", "Home"));
					}
				}
			}

			return View();
		}

		public IActionResult LogOut()
		{
			HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "KhachHang");
		}

       
        public IActionResult Profile()
		{
			CustomerModel customer = _dataContext.customers.FirstOrDefault(p => p.Email == User.Identity.Name);

            //danh sách yêu thích vui
            // Lấy ID người dùng đang đăng nhập
            var customerEmail = User.Identity.Name;
            var customerId = _dataContext.customers
             .Where(c => c.Email == customerEmail)
             .Select(c => c.Id)
             .FirstOrDefault();
			var favorite = _dataContext.favoriteProducts
				.Where(f => f.CustomerId == customerId)
				.ToList();
			var favoritelist = from f in favorite
							   join p in _dataContext.products on f.ProductId equals p.Id
							   select new ProductList
                               {
								   Id=p.Id,
							   Image=p.Image,
							   Name=p.Name,
							   Price=p.Price
							   };
            ViewBag.AlertMessage = customerEmail;
			ViewBag.favoritelist = favoritelist.ToList();
          
            return View(customer);
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
