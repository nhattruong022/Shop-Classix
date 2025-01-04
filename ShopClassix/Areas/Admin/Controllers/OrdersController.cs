using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;
using System.Linq;

namespace Shop_Classix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly DataContext _dataContext;
        private const int PageSize = 10;

        public OrdersController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("Admin/Orders")]
        public async Task<IActionResult> Orders(int page = 1, string search = null)
        {
            var ordersQuery = _dataContext.orders.Include(o => o.customers).Where(o => o.Status != 0);

            if (!string.IsNullOrEmpty(search))
            {
                ordersQuery = ordersQuery.Where(o => o.Id.ToString().Contains(search));
            }

            var totalOrders = await ordersQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

            var orders = await ordersQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrder(int id)
        {
            var order = await _dataContext.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == 2 && order.PaymentMethod == 1)
                order.Status = 4;
            else
                order.Status++;

            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Orders");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _dataContext.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = 0;
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Orders");
        }
    }
}