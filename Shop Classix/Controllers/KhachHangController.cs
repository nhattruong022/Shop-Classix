using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Shop_Classix.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly DataContext _dataContext;

        public KhachHangController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu email đã tồn tại
                var accountExist = _dataContext.customers.FirstOrDefault(p => p.Email == model.email);

                if (accountExist != null)
                {
                    ModelState.AddModelError("", "Email already exists.");
                    return View(model);
                }

                // Mã hóa mật khẩu
                var passwordHasher = new PasswordHasher<CustomerModel>();
                var hashedPassword = passwordHasher.HashPassword(null, model.password);

                var user = new CustomerModel
                {
                    Name = model.Hoten,
                    Email = model.email,
                    PhoneNumber = model.phoneNumber,
                    Password = hashedPassword, // Đảm bảo mật khẩu đã mã hóa được lưu vào database
                    Address = model.diachi,
                    Gender = model.gioitinh ?? false, // Giả sử là 'false' nếu chưa chọn
                    DateOfBirth = model.NgaySinh ?? DateTime.Now, // Giá trị mặc định nếu không có
                };

                // Thêm người dùng vào cơ sở dữ liệu
                _dataContext.customers.Add(user);
                _dataContext.SaveChanges();

                // Chuyển hướng người dùng sau khi đăng ký thà  nh công
                return RedirectToAction("Login","KhachHang");
            }

            // Nếu ModelState không hợp lệ, trả lại form và hiển thị lỗi
            return View(model);
        }



        public IActionResult Login()
        {
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
