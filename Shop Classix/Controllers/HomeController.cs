using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using System.Diagnostics;
using X.PagedList.Extensions;


namespace Shop_Classix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext dataContext;
        public HomeController(ILogger<HomeController> logger,DataContext _datacontext)
        {
            dataContext=_datacontext;
            _logger = logger;
        }

        public IActionResult Index(int? categoryId)
        {  

            // Lấy tất cả các danh mục để hiển thị trên giao diện
            ViewBag.categories = new SelectList(dataContext.categories, "Id", "Name");

            // Gửi giá trị categoryId vào ViewBag để hiển thị trong View
            ViewBag.SelectedCategoryId = categoryId;

            // Lọc sản phẩm theo danh mục nếu có categoryId, nếu không lấy tất cả sản phẩm
            var products = dataContext.products.Include(p => p.category) // Lấy thông tin category cùng với sản phẩm
                                              .Where(p => !categoryId.HasValue || p.CategoryId == categoryId) // Kiểm tra categoryId có khớp không
                                              .ToList();

      
          

            return View(products);
        }




        public IActionResult TimKiem(string keyword, int? categoryId,int? price,int ?page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);


            
            var products = dataContext.products.Include(p => p.category).AsQueryable();

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p => p.Name.Contains(keyword));
            }

            if(price.HasValue)
            {
                switch(price.Value)
                {
                    case 1:
                        products = products.Where(p => p.Price < 100000);
                        break;
                    case 2:
                        products = products.Where(p => p.Price >= 100000 && p.Price <= 500000);
                        break;
                    case 3:
                        products = products.Where(p => p.Price > 500000);
                        break;
                }    
            }


            //tìm kiếm phân trang sắp xếp theo id
            var pagedProducts= products.OrderBy(p => p.Id).ToPagedList(pageNumber, pageSize);


            //truyền danh mục và tham số tìm kiếm vào viewBag để sử dụng cho phần phân trang
            ViewBag.keyword = keyword;
            ViewBag.categoryId = categoryId;
            ViewBag.price = price;

            ViewBag.categories = new SelectList(dataContext.categories, "Id", "Name");

            // Trả về kết quả tìm kiếm
            return View("TimKiem",pagedProducts);
        }


        [HttpPost]
        public IActionResult ToggleFavorite(int productId)
        {
            var product = dataContext.products.SingleOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            // Chuyển đổi trạng thái yêu thích
            product.IsFavorite = !product.IsFavorite;

            // Cập nhật FavoriteNumber
            product.FavoriteNumber = product.IsFavorite ? 1 : 0;

            dataContext.SaveChanges();

            return Json(new
            {
                success = true,
                isFavorite = product.IsFavorite,
                favoriteCount = product.IsFavorite ? 1 : 0 // trả về 1 nếu yêu thích, 0 nếu không
            });
        }







        public IActionResult DetailProduct()    
        {
            return View();
        }

        




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
