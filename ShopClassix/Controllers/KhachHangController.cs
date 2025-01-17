using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
                // kiểm tra nếu email tồn tại trong database
                var khachHang = _dataContext.customers.SingleOrDefault(kh => kh.Email == model.Email);

                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "This customer does not exist");
                    return View();
                }

                // kiểm tra nếu password đúng
                var passwordHasher = new PasswordHasher<CustomerModel>();  
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(khachHang, khachHang.Password, model.Password); //mẫ hóa mật khẩu

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("loi", "Password is not correct");
                    return View();
                }
                else
                {
                    // Create the claims based on user role
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, khachHang.Email),  // Email
                new Claim(ClaimTypes.NameIdentifier, khachHang.Id.ToString()),  // Customer Id
                new Claim(ClaimTypes.Name, khachHang.Email),  // Display name
                new Claim(ClaimTypes.Role, khachHang.Role ?? "User")  // Role (Admin or User)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Authentication properties
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,  //Cookie tồn tại sau khi trình duyệt đóng 
                        RedirectUri = returnUrl ?? Url.Action("Index", "Home")  // Redirect to the appropriate page
                    };


                    // Add cookies for User and Admin
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = false, // Có thể truy cập từ JavaScript
                        Secure = true,    // Chỉ gửi qua HTTPS
                        Expires = DateTime.UtcNow.AddDays(7), // Cookie hết hạn sau 7 ngày
						SameSite = SameSiteMode.Lax  // or SameSiteMode.Strict depending on your use case
                    };

                    // Check if the user is an Admin or User
                    if (khachHang.Role == "Admin")
                    {
                        // Sign-in with Admin cookie
                        HttpContext.SignInAsync("AdminCookie", new ClaimsPrincipal(claimsIdentity), authProperties).Wait();

                        return RedirectToAction("Index", "Home", new { area = "Admin" });  // Redirect to Admin area
                                                         
                    }
                    else
                    {
                        // Sign-in with User cookie
                        HttpContext.SignInAsync("UserCookie", new ClaimsPrincipal(claimsIdentity), authProperties).Wait();

                        return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                       
                    }
                }
            }

            return View();
        }


		//đăng xuát
        public IActionResult LogOut(string role)
        {
            string authScheme = role == "Admin" ? "AdminCookie" : "UserCookie";

            HttpContext.SignOutAsync(authScheme); // Đảm bảo xóa đúng cookie
            return RedirectToAction("Login", "KhachHang"); // Chuyển về trang login
        }



        //[Authorize]
        [Authorize(Policy = "UserOnly")]
        public IActionResult Profile(int id)
		{
            

            CustomerModel customer = _dataContext.customers.FirstOrDefault(p => p.Email == User.Identity.Name);

			//danh sách yêu thích vui
			// Lấy ID người dùng đang đăng nhập
			var customerEmail = User.Identity.Name;
			var customerId = _dataContext.customers
			 .Where(c => c.Email == customerEmail)
			 .Select(c => c.Id)
			 .FirstOrDefault();
            if (id != 0)
            {
                var productf = _dataContext.favoriteProducts.FirstOrDefault(p => p.ProductId == id && p.CustomerId == customerId);
                if (productf != null)
                {
                    // Xóa sản phẩm yêu thích
                    _dataContext.favoriteProducts.Remove(productf);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _dataContext.SaveChanges();
                }
            }
            var favorite = _dataContext.favoriteProducts
				.Where(f => f.CustomerId == customerId)
				.ToList();
           
            var favoritelist = from f in favorite
							   join p in _dataContext.products on f.ProductId equals p.Id
							   select new ProductList
							   {
								   Id = p.Id,
								   Image = p.Image,
								   Name = p.Name,
								   Price = p.Price,
								   Rating = p.Rating
							   };
			ViewBag.AlertMessage = customerEmail;
			ViewBag.favoritelist = favoritelist.ToList();

			

            return View(customer);
		}


		public async Task<IActionResult> MyOrder(int? status, int page = 1)
		{
			const int PageSize = 5;

			var customerEmail = User.Identity.Name;
			var customer = await _dataContext.customers
				.FirstOrDefaultAsync(c => c.Email == customerEmail);

			if (customer == null)
			{
				return NotFound();
			}

			var filteredOrders = _dataContext.orders
				.Where(o => o.CustomerId == customer.Id && (!status.HasValue || o.Status == status))
				.OrderByDescending(o => o.CreateAt);

			var totalOrders = await filteredOrders.CountAsync();
			var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

			var orders = await filteredOrders
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToListAsync();
			
			ViewBag.Status = status;
			ViewBag.TotalPages = totalPages;
			ViewBag.CurrentPage = page;
			ViewBag.Orders = orders;

			return View();
		}

		public async Task<IActionResult> MyOrderDetail(int orderId)
		{
			var orderDetail = await _dataContext.orderDetails
				.Where(o => o.OrderId == orderId)
				.Include(o => o.Products)
				.ToListAsync();

			if (orderDetail == null)
			{
				return NotFound();
			}
			ViewBag.Detail = orderDetail;
			return View();
		}

		public async Task<IActionResult> CancelOrder(int orderId)
		{
			var order = await _dataContext.orders.FindAsync(orderId);
			if (order == null)
			{
				return NotFound();
			}

			order.Status = 4; // Canceled
			await _dataContext.SaveChangesAsync();

			return RedirectToAction("MyOrder");
		}
		public IActionResult Comments(int status)
		{

			//vui

			var customerEmail = User.Identity.Name;
			var customerId = _dataContext.customers
			 .Where(c => c.Email == customerEmail)
			 .Select(c => c.Id)
			 .FirstOrDefault();

			var comment = from p in _dataContext.products
						  join pc in _dataContext.productComments on p.Id equals pc.ProductId
						  select new
						  {
							  ID = pc.AccountId,
							  Images = p.Image,
							  Names = p.Name,
							  Content = pc.Cotent,
							  Ratings = pc.Rating,
							  IdProduct = pc.ProductId
						  };
			ViewBag.status = status;
			if (status == 1) { return View(); }
			if (status == 2)
			{
				var comments = comment.Where(c => c.ID == customerId);
				if (comments.Any()) { ViewBag.comment = comments; }
				else { ViewBag.comment = null; }
				return View();
			}

			return View();
		}
	}
}
