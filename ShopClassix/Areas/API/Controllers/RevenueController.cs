using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;

namespace Shop_Classix.Areas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public RevenueController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        int currentYear = DateTime.Now.Year; // Lấy năm hiện tại
        int currentMonth = DateTime.Now.Month; // Lấy tháng hiện tại
        [HttpGet("revenuemonth")]
        public async Task<JsonResult> GetValueMonth(int month, int year)
        {


            var valuemonth = await GetMonthsales(year, month);
            return new JsonResult(valuemonth);
        }

        [HttpGet("getrevenue")]
        public async Task<JsonResult> GetMonthsales(int year, int month)
        {
            var orderdetail = from o in _dataContext.orders
                              join d in _dataContext.orderDetails on o.Id equals d.OrderId
                              join p in _dataContext.products on d.ProductId equals p.Id
                              select new
                              {
                                  Order = o.Id,
                                  OdName = p.Name,
                                  Odstatus = o.Status,
                                  Productid = d.ProductId,
                                  Quantitys = d.Quantity,
                                  OdCreateAt = o.CreateAt,
                                  Odtotalprice = d.TotalPrice,
                                  OdTotal = o.TotalPrice,
                                  Odoriginalprice = p.originalprice,
                              };

            var totalRevenue = await orderdetail
                .Where(o => o.Odstatus == 3 &&
                             o.OdCreateAt.HasValue &&
                             o.OdCreateAt.Value.Year == year &&
                             o.OdCreateAt.Value.Month == month)
                .SumAsync(o => o.OdTotal);

            var monthlySales = await orderdetail
                .Where(o => o.Odstatus == 3 &&
                             o.OdCreateAt.HasValue &&
                             o.OdCreateAt.Value.Year == year &&
                             o.OdCreateAt.Value.Month == month)
                .GroupBy(o => new { o.Productid })
                .Select(g => new ProductSales
                {
                    Year = year,
                    Month = month,
                    ProductType = g.Key.Productid.ToString(),
                    Name = g.Select(o => o.OdName).FirstOrDefault(),
                    QuantitySold = g.Sum(o => o.Quantitys),
                    TotalCapital = g.Sum(o => o.Odoriginalprice * o.Quantitys),
                    TotalSales = g.Sum(o => o.Odtotalprice),
                    TotalProfit = g.Sum(o => o.Odtotalprice) - g.Sum(o => o.Odoriginalprice * o.Quantitys),
                    Revenue = totalRevenue
                }).ToListAsync();

            return new JsonResult(monthlySales);
        }


        [HttpGet("getyear")]
        public async Task<JsonResult> GetYear(int oldYear, int thisYear)
        {
            if (oldYear < 2020 || oldYear > DateTime.Now.Year || thisYear < 2020 || thisYear > DateTime.Now.Year)
            {
                // Trả về lỗi nếu năm không hợp lệ
                return new JsonResult(new { error = "Invalid year selected." }) { StatusCode = 400 };
            }

            var revenuesThisYear = await GetMonthlyRevenueAsync(thisYear);
            revenuesThisYear = FillMissingMonths(revenuesThisYear);

            var revenuesLastYear = await GetMonthlyRevenueAsync(oldYear);
            revenuesLastYear = FillMissingMonths(revenuesLastYear);

            return new JsonResult(new { thisYear = revenuesThisYear, lastYear = revenuesLastYear });
        }


        private async Task<List<double>> GetMonthlyRevenueAsync(int year)
        {
            var monthlyRevenues = await _dataContext.orders
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
                result[i] = revenues[i]; // Gán doanh thu tại chỉ số tương ứng
            }

            return result;
        }

    }
}