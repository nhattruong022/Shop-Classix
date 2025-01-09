using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
