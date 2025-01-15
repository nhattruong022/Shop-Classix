using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;
using Microsoft.EntityFrameworkCore;

namespace Shop_Classix.Areas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("user")]
        public async Task<JsonResult> GetOrderUser(int? status, int page = 1)
        {
            const int PageSize = 5;

            var customerEmail = User.Identity.Name;
            var customer = await _dataContext.customers
                .FirstOrDefaultAsync(c => c.Email == customerEmail);

            var filteredOrders = _dataContext.orders
                .Where(o => o.CustomerId == customer.Id && (!status.HasValue || o.Status == status))
                .OrderByDescending(o => o.CreateAt);

            var totalOrders = await filteredOrders.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

            var orders = await filteredOrders
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var Status = status;
            return new JsonResult(new { orders, Status });
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetOrderAdmin(int page = 1, string? search = null, int? status = null)
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

            var totalOrders = await ordersQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

            var orders = await ordersQuery
                .OrderBy(o =>
                    o.Status == 4 ? 0 :
                    o.Status == 1 ? 1 :
                    o.Status == 2 ? 2 :
                    o.Status == 3 ? 3 :
                    o.Status == 5 ? 4 : 5) 
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var Status = status;
            var Search = search;

            return new JsonResult(new { orders, Status, Search });
        }
    }
}
