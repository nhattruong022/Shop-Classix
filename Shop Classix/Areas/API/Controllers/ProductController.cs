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

    }
}
