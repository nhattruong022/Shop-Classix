using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Microsoft.Identity.Client;
namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _datacontext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        int currentYear = DateTime.Now.Year; // Lấy năm hiện tại
        int currentMonth = DateTime.Now.Month; // Lấy tháng hiện tại

        public DashboardController(DataContext datacontext, IWebHostEnvironment hostEnvironment)
        {
            _datacontext = datacontext;
            _webHostEnvironment = hostEnvironment;
        }
        public class ProductSales
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string Name {  get; set; }
            public string ProductType { get; set; }
            public int QuantitySold { get; set; }
            public double TotalSales {  get; set; }
            public double Revenue { get; set; }
            public double TotalProfit { get; set; }
            public double TotalCapital {  get; set; }
        }

        [HttpGet("Admin/Dashboard/Chart")]
        public async Task<IActionResult> Chart(int oldyear, int thisyear, int month, int year)
        {
            if (thisyear == 0)
            {
                thisyear = currentYear;
                oldyear = thisyear - 1;
            }
            else if (oldyear == 0)
            {
                oldyear = thisyear - 1;
            }

            var revenuesThisYear = await GetMonthlyRevenueAsync(thisyear);
            var revenuesLastYear = await GetMonthlyRevenueAsync(oldyear);

            ViewBag.RevenuesThisYear = FillMissingMonths(revenuesThisYear);
            ViewBag.RevenuesLastYear = FillMissingMonths(revenuesLastYear);

            // Chờ cho phương thức GetMonthlySales hoàn tất
        
         var RevenuesMonth = await GetMonthlySales(year, month);
            if (RevenuesMonth.Any())
            {
                ViewBag.RevenuesMonth = RevenuesMonth;
            }
            else
            {
                ViewBag.RevenuesMonth = null;
            }

            return View();
        }

        private async Task<List<ProductSales>> GetMonthlySales(int year, int month)
        {
            if (month != 0 && year != 0)
            {
                return await GetMonthsales(year, month);
            }
            else if (year == 0 && month != 0)
            {
                return await GetMonthsales(currentYear, month);
            }
            else if (month == 0 && year != 0)
            {
                return await GetMonthsales(year, currentMonth);
            }
            else
            {
                return await GetMonthsales(currentYear, currentMonth);
            }
        }
        private async Task<List<double>> GetMonthlyRevenueAsync(int year)
        {
            var monthlyRevenues = await _datacontext.orders
        .Where(o => o.Status == 3 && o.CreateAt.HasValue && o.CreateAt.Value.Year == year)
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

        private async Task<List<ProductSales>> GetMonthsales(int year, int month)
        {
            var orderdetail = from o in _datacontext.orders
                              join d in _datacontext.orderDetails on o.Id equals d.OrderId
                              join p in _datacontext.products on d.ProductId equals p.Id
                              select new {
                              Order=o.Id,
                              OdName=p.Name,
                              Odstatus=o.Status,
                              Productid=d.ProductId,
                              Quantitys=d.Quantity,
                              OdCreateAt=o.CreateAt,
                              Odtotalprice=d.TotalPrice,
                              OdTotal=o.TotalPrice,
                                  Odoriginalprice=p.originalprice,

                              };
            // Đảm bảo orderdetail là danh sách hoặc IQueryable
            var totalRevenue = orderdetail
     .Where(o => o.Odstatus == 3 &&
                  o.OdCreateAt.HasValue &&
                  o.OdCreateAt.Value.Year == year &&
                  o.OdCreateAt.Value.Month == month)
     .Sum(o => o.OdTotal);

            ViewBag.Revenues = totalRevenue;
            var monthlySales =await orderdetail
                .Where(o=>o.Odstatus==3 && o.OdCreateAt.HasValue&& o.OdCreateAt.Value.Year==year&& o.OdCreateAt.Value.Month == month)
                .GroupBy(o => new { o.Productid })
               .Select(g => new ProductSales
               {
                   Year = year,
                   Month = month,
                   ProductType = g.Key.Productid.ToString(), // Hoặc sử dụng thông tin loại sản phẩm phù hợp
                   Name = g.Select(o => o.OdName).FirstOrDefault(),
                   QuantitySold = g.Sum(o => o.Quantitys), // Tính tổng số lượng bán được
                  TotalCapital= g.Sum(o => o.Odoriginalprice * o.Quantitys),
                   TotalSales = g.Sum(o=>o.Odtotalprice), // Cần tính giá trị này nếu có
                   TotalProfit = g.Sum(o => o.Odtotalprice)- g.Sum(o => o.Odoriginalprice * o.Quantitys)

               })
    .ToListAsync();
            return (monthlySales);

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
