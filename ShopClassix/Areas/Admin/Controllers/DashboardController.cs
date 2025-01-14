using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _datacontext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(DataContext datacontext, IWebHostEnvironment hostEnvironment)
        {
            _datacontext = datacontext;
            _webHostEnvironment = hostEnvironment;
        }


        [HttpGet("Admin/Dashboard/Chart")]
        public async Task<IActionResult> Chart()
        {
            var revenuesThisYear = await GetMonthlyRevenueAsync(2025);
            var revenuesLastYear = await GetMonthlyRevenueAsync(2024);

            // Gán giá trị 0 cho các tháng không có doanh thu
            ViewBag.RevenuesThisYear = FillMissingMonths(revenuesThisYear);
            ViewBag.RevenuesLastYear = FillMissingMonths(revenuesLastYear);
            return View();
        }
        private async Task<List<double>> GetMonthlyRevenueAsync(int year)
        {
            var monthlyRevenues = await _datacontext.orders
        .Where(o => o.Status == 4 && o.CreateAt.HasValue && o.CreateAt.Value.Year == year)
        .GroupBy(o => o.CreateAt.Value.Month)
        .Select(g => new { Month = g.Key, Total = g.Sum(o => o.TotalPrice) })
        .OrderBy(m => m.Month)
        .ToListAsync();

            // Khởi tạo danh sách doanh thu với 12 giá trị 0
            var revenues = new List<double>(new double[12]);

            // Gán doanh thu cho các tháng tương ứng
            foreach (var revenue in monthlyRevenues)
            {
                revenues[revenue.Month - 1] = revenue.Total; // Gán giá trị vào đúng chỉ số
            }

            return revenues;
        }
        private List<double> FillMissingMonths(List<double> revenues)
        {
            // Khởi tạo danh sách với 12 giá trị 0
            var result = new List<double>(new double[12]);

            // Gán doanh thu cho các tháng tương ứng
            for (int i = 0; i < revenues.Count; i++)
            {
                // Giả sử revenues[i] là doanh thu cho tháng i + 1
                result[i] = revenues[i]; // Gán doanh thu tại chỉ số tương ứng
            }

            return result;
        }


        public class MonthlyRevenue
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal TotalRevenue { get; set; }
        }
        //het vui

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

            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        //Edit contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var exist_contact = await _datacontext.contacts.FirstOrDefaultAsync();
            if (exist_contact == null)
            {
                ModelState.AddModelError("", "Contact is not exist");
                return View(contact);
            }


            if (ModelState.IsValid)
            {
                if (contact.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Classix/img/logo");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    String imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(contact.ImageUpload.FileName);
                    string filePath = Path.Combine(uploadDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
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
                return RedirectToAction("Contact", "Dashboard");
            }
            return View(contact);
        }

        //[HttpGet("Admin/Dashboard/Comments")]
        //public IActionResult Comments()
        //{
        //    return View();
        //}
        [HttpGet("Admin/Dashboard/Comments")]
        public IActionResult Comments()
        {
            var comments = _datacontext.productComments
                .Include(pc => pc.customers)
                .Include(pc => pc.products)
                .ThenInclude(p => p.category)
                .Select(pc => new CommentViewModel
                {
                    CommentId = pc.Id,
                    CustomerName = pc.customers.Name,
                    ProductName = pc.products.Name,
                    ProductImage = pc.products.Image,
                    Category = pc.products.category.Name,
                    Content = pc.Cotent,
                    Rating = pc.Rating
                }).ToList();

            return View(comments);
        }

        [HttpPost("Admin/Dashboard/DeleteComment/{id}")]
        public IActionResult DeleteComment(int id)
        {
            // Tìm bình luận theo ID
            var comment = _datacontext.productComments.Find(id);

            if (comment != null)
            {
                // Xóa bình luận
                _datacontext.productComments.Remove(comment);
                _datacontext.SaveChanges(); // Lưu thay đổi vào CSDL
            }

            // Chuyển hướng về trang danh sách bình luận
            return RedirectToAction("Comments");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete()
        {
            var record = _datacontext.contacts.FirstOrDefault();
            if (record == null)
            {
                return NotFound();
            }

            _datacontext.contacts.Remove(record);
            _datacontext.SaveChanges();
            return RedirectToAction("Contact"); // Redirect to the list page after deletion
        }
    }
}
