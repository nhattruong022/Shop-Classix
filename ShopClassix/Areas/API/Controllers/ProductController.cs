using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;

namespace Shop_Classix.Areas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public JsonResult GetProducts()
        {
            var product = _dataContext.products;
            var categories = _dataContext.categories;
            var ProductWithCategories = product.Join(
                categories,
                p => p.CategoryId,
                c => c.Id,
                (p, c) => new { Id = p.Id, Name = p.Name, Image = p.Image, Price = p.Price, Description = p.Description, Slug = p.Slug, Status = p.Status, CateName = c.Name }
       );

            return new JsonResult(ProductWithCategories);
        }
        [HttpGet("{id}/viewcount")]
        public ActionResult<int> GetViewCount(int id)
        {
            var product = _dataContext.products.Find(id);
            if (product == null) return NotFound();
            var views = product.Views;
            return new JsonResult(views);
        }
        [HttpGet("{id}/favorite")]
        public ActionResult<int> GetFavorite(int id)
        {
            var favorite = 0;
            var product = _dataContext.products.Find(id);
            if (product.FavoriteNumber.HasValue)
            {
                favorite = product.FavoriteNumber.Value;
            }

            return new JsonResult(favorite);
        }

        [HttpGet("{id}/quantitys")]
        public ActionResult<int> GetQuantity(int id)
        {
            var quantity = 0;
            var product = _dataContext.products.Find(id);
            if (product.Quantity.HasValue)
            {
                quantity = product.Quantity.Value;
            }

            return new JsonResult(quantity);

        }
    }
}
