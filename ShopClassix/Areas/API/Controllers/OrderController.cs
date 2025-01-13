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
    }
}
