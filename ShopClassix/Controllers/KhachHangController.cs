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
					var productfa = _dataContext.products.Where(p => p.Id == id).FirstOrDefault();
					productfa.FavoriteNumber--;
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
			ViewBag.number = favoritelist.Count();

			

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
			return Json(orderDetail);
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

			return Json(new { success = true });
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
