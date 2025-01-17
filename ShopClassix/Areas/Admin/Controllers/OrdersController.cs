using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;
using System.Linq;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class OrdersController : Controller
    {
        private readonly DataContext _dataContext;

        public OrdersController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }




        //Hiển thị danh sách

        [HttpGet("Admin/Orders")]
        public async Task<IActionResult> Orders(int page = 1, string? search = null, int? status = null)
        {
            const int PageSize = 10;
            var ordersQuery = _dataContext.orders.AsQueryable();

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.Status == status);
            }

            // Lọc theo tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                ordersQuery = ordersQuery.Where(o => o.Id.ToString().Contains(search));
            }

            //Phân trang
            var totalOrders = await ordersQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

            var orders = await ordersQuery
                .OrderBy(o =>
                    o.Status == 4 ? 0 :
                    o.Status == 1 ? 1 :
                    o.Status == 2 ? 2 :
                    o.Status == 3 ? 3 :
                    o.Status == 5 ? 4 : 5) // Thay đổi theo độ ưu tiên trạng thái
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(orders);
        }

        //Thay đổi trạng thái
        [HttpPost]
        public async Task<IActionResult> ChangeOrder(int id)
        {
            var order = await _dataContext.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status++;
            await _dataContext.SaveChangesAsync();

            return Json(new { success = true, message = "Order status updated successfully." });
        }

        //Xóa đơn hàng
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _dataContext.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = 5;
            await _dataContext.SaveChangesAsync();

            return Json(new { success = true, message = "Order canceled successfully." });
        }
    }
}