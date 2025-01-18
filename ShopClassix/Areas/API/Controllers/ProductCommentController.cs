using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shop_Classix.Models;
using Shop_Classix.Repository;

namespace Shop_Classix.Areas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCommentController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductCommentController(DataContext dataContext, IHubContext<ProductHub> hubContext)
        {
            _dataContext = dataContext;
            _hubContext = hubContext;
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

        // Thêm bình luận cho sản phẩm
        [HttpPost("{productId}/comment")]
        public async Task<IActionResult> PostComment(int productId, [FromBody] ProductCommentModel model)
        {
            // Lấy userId từ User.Identity.Name hoặc từ Claims
            var userIdString = User.Identity.Name ?? User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Chuyển userId từ string sang int
            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid userId.");
            }

            // Kiểm tra người dùng đã mua sản phẩm hay chưa
            var purchaseResult = await VerifyPurchase(userId, productId) as OkObjectResult;

            if (purchaseResult?.Value is JObject response &&
                response["canReview"] != null &&
                (bool)response["canReview"])
            {
                // Thêm bình luận vào cơ sở dữ liệu
                var productComment = new ProductCommentModel
                {
                    ProductId = productId,
                    AccountId = userId,
                    Rating = model.Rating,
                    Content = model.Content,
                    CreatedAt = DateTime.Now
                };

                _dataContext.productComments.Add(productComment);
                await _dataContext.SaveChangesAsync();

                // Cập nhật số lượng bình luận và điểm đánh giá
                var reviewCount = _dataContext.productComments.Count(rc => rc.ProductId == productId);
                var averageRating = await _dataContext.productComments
                    .Where(rc => rc.ProductId == productId)
                    .AverageAsync(rc => (double?)rc.Rating);

                // Gửi thông báo cập nhật cho tất cả client qua SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveProductCount", reviewCount);
                await _hubContext.Clients.All.SendAsync("ReceiveProductRating", averageRating ?? 0.0);

                return Ok(new { message = "Comment posted successfully!" });
            }

            return BadRequest("You are not eligible to review this product.");
        }
    }
}
