using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Repository;

namespace Shop_Classix.Areas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCommentController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ProductCommentController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public JsonResult GetProductComment()
        {
            var comment = _dataContext.productComments;
            return new JsonResult(comment);
        }

        [HttpGet("{id}/reviews")]
        public ActionResult<int> GetReviews(int id)
        {
            var count = 0;
            // Đếm tổng số lượng comment trong bảng productComments
            var reviewCount = _dataContext.productComments
                .Where(rc => rc.ProductId == id);
               if (reviewCount.Any())
            {
                 count= reviewCount.Count();
            }
           


            return new JsonResult(count);
        }


        [HttpGet("{id}/rating")]
        public ActionResult<int> GetRating(int id)
        {
            // if(_dataContext.productComments.Ratinf)
            // Tính trung bình rating cho sản phẩm có ProductId tương ứng
            double rating = 0;
            var averageRating = _dataContext.productComments
                .Where(rc => rc.ProductId == id);
            if (averageRating.Any()) {
               rating= averageRating
                .Average(rc => rc.Rating);
            }
           

            return new JsonResult(rating);
        }

        // Xác thực người dùng đã mua sản phẩm để được phép đánh giá
        [HttpGet("{userId}/{productId}/verify-purchase")]
        public async Task<IActionResult> VerifyPurchase(int userId, int productId)
        {
            var purchaseExists = await _dataContext.orders
                .AnyAsync(order => order.CustomerId == userId &&
                                   order.Status == 4 &&  // Đơn hàng đã giao thành công
                                   order.orderDetails.Any(od => od.ProductId == productId));  // Sản phẩm đã được mua

            return Ok(new { canReview = purchaseExists });
        }
    }
}
