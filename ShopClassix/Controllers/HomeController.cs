using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Models.ViewModels;
using Shop_Classix.Repository;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList.Extensions;
using Shop_Classix.Helper;
using Microsoft.CodeAnalysis;
using NuGet.Protocol.Plugins;

namespace Shop_Classix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext dataContext;
        public HomeController(ILogger<HomeController> logger, DataContext _datacontext)
        {
            dataContext = _datacontext;
            _logger = logger;
        }

        public IActionResult Index(int? categoryId)
        {
            // Lấy thông tin user hiện tại
            var userId = User.Identity.IsAuthenticated
                                                   ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
                                                   : (int?)null;

            // Lấy tất cả các danh mục để hiển thị trên giao diện
            ViewBag.categories = new SelectList(dataContext.categories, "Id", "Name");

            // Gửi giá trị categoryId vào ViewBag để hiển thị trong View
            ViewBag.SelectedCategoryId = categoryId;

            // Lọc sản phẩm theo danh mục nếu có categoryId, nếu không lấy tất cả sản phẩm
            var AllProducts = dataContext.products.Include(p => p.category) // Lấy thông tin category cùng với sản phẩm
                                                 .Where(p => !categoryId.HasValue || p.CategoryId == categoryId) // Kiểm tra categoryId có khớp không
                                                 .ToList();

            // Lấy danh sách sản phẩm yêu thích
            var favoriteProducts = dataContext.favoriteProducts
                                               .Where(fp => fp.CustomerId == userId)
                                               .Include(fp => fp.products)
                                               .Select(fp => fp.products)
                                               .ToList();


            //lấy danh sách sản phẩm bán chạy sau khi thanh toán thành công
            var bestSellingProduct = dataContext.orderDetails.GroupBy(od => od.ProductId).Select(group => new
            {
                ProductId = group.Key,
                TotalSold = group.Sum(od => od.Quantity)
            }).OrderByDescending(x => x.TotalSold) //sắp xếp giảm dần
            .Take(4) //giới hạn 4 sản phẩm bán chạy
            .Join(dataContext.products,  //join với bảng products
                  od => od.ProductId,
                  p => p.Id,
                  (od, p) => new ProductsModel
                  {
                      Id = p.Id,
                      Name = p.Name,
                      Price = p.Price,
                      Image = p.Image,
                      Quantity = od.TotalSold,
                      Rating = p.Rating
                  }).ToList();

            ViewBag.BestSellingProduct = bestSellingProduct;


            //lấy danh sách sản phẩm cuối cùng được thêm vào
            var lastestProducts = dataContext.products.OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .Select(p => new ProductsModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    CreatedAt = p.CreatedAt,
                    Rating = p.Rating
                }).ToList();


            ViewBag.LastestProducts = lastestProducts;



            // Tạo ViewModel để gộp cả danh sách yêu thích và tất cả sản phẩm
            var model = new ProductPageViewModel
            {
                AllProducts = AllProducts,
                FavoriteProducts = favoriteProducts
            };

            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            ViewBag.UniqueProductCount = cart.Items.Select(item => item.ProductId).Distinct().Count();
            //lấy dữ liệu footer vui
            var contacts = dataContext.contacts
       .Select(c => new ContactModel
       {
           Name = c.Name,

           Email = c.Email,
           PhoneNumber = c.PhoneNumber,
           Address = c.Address,
           Logo = c.Logo
       })
         .ToList();

            if (contacts != null && contacts.Any())
            {
                var firstContact = contacts.FirstOrDefault();
                HttpContext.Session.SetString("Address", firstContact.Address);
                HttpContext.Session.SetString("Phone", firstContact.PhoneNumber);
                HttpContext.Session.SetString("Email", firstContact.Email);
                HttpContext.Session.SetString("Logo", firstContact.Logo);
<<<<<<< HEAD
            }


=======
            }
>>>>>>> d8e92bee1560337cc4f0903e8563d41a09888e01
            ViewBag.UniqueProductCount = cart.Items.Select(item => item.ProductId).Distinct().Count();
            ViewBag.TotalAmount = cart.TotalAmount;

            return View(model);

        }


        public IActionResult Details(int id)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu theo Id
            var product = dataContext.products
                .Include(p => p.category)
                .FirstOrDefault(p => p.Id == id);

            // Kiểm tra xem sản phẩm có tồn tại không
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm, trả về lỗi 404
            }

            // Lấy danh sách sản phẩm liên quan cùng danh mục, loại trừ sản phẩm hiện tại
            var relatedProducts = dataContext.products
                .Include(p => p.category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(4)
                .ToList();

            // Gán danh sách sản phẩm liên quan vào ViewBag
            ViewBag.RelatedProducts = relatedProducts;
            // Kiểm tra session để xem lượt xem đã được tăng chưa vui
            var sessionKey = $"ProductView_{id}";
            if (HttpContext.Session.GetInt32(sessionKey) == null)
            {
                // Tăng lượt xem
                product.Views++;
                dataContext.SaveChanges();

                // Lưu thông tin vào session để không tăng lượt xem trong phiên này
                HttpContext.Session.SetInt32(sessionKey, 1);
            }
            ViewBag.view = product.Views;
            ViewBag.productid = product.Id;
            var reviewCount = dataContext.productComments.Count();
            // Gán giá trị vào ViewBag
            ViewBag.reviews = reviewCount > 0 ? reviewCount : 0;
            ViewBag.favorite = product.FavoriteNumber;
            var productcomment = dataContext.productComments
             .Where(rt => rt.ProductId == product.Id);
            double rating = 5;
            if (productcomment.Any())
            {
                rating = productcomment
                  .Average(rc => rc.Rating);

            }
            ViewBag.rating = rating;



            return View(new List<Shop_Classix.Models.ProductsModel> { product });
        }


        public IActionResult TimKiem(string keyword, int? categoryId, int? price, int? page)
        {
            int pageSize = 4;   // số sản phẩm trong 1 trang
            int pageNumber = (page ?? 1);  // mặc định là trang 1

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

            // Lọc theo giá
            if (price.HasValue)
            {
                switch (price.Value)
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

            // Phân trang và sắp xếp theo ID
            var pagedProducts = products.OrderBy(p => p.Id).ToPagedList(pageNumber, pageSize);

            // Truyền danh mục và tham số tìm kiếm vào ViewBag để sử dụng cho phần phân trang
            ViewBag.keyword = keyword;
            ViewBag.categoryId = categoryId;
            ViewBag.price = price;
            ViewBag.categories = new SelectList(dataContext.categories, "Id", "Name");

            // Trả về kết quả tìm kiếm
            return View("TimKiem", pagedProducts);
        }


        //thêm sản phẩm yêu thích
        [Authorize]
        [HttpPost]
        public IActionResult ToggleFavorite(int productId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var favoriteProduct = dataContext.favoriteProducts
                .SingleOrDefault(fp => fp.CustomerId == userId && fp.ProductId == productId);


            if (favoriteProduct == null)
            {
                // Thêm sản phẩm vào danh sách yêu thích nếu chưa có
                favoriteProduct = new FavoriteProductModel
                {
                    CustomerId = userId,
                    ProductId = productId,


                };
                dataContext.favoriteProducts.Add(favoriteProduct);
            }
            else
            {
                // Xóa sản phẩm khỏi danh sách yêu thích nếu đã có
                dataContext.favoriteProducts.Remove(favoriteProduct);
            }

            dataContext.SaveChanges();

            // Đếm số lượng yêu thích của sản phẩm
            var favoriteCount = dataContext.favoriteProducts.Count(fp => fp.ProductId == productId);

            return Json(new
            {
                success = true,
                isFavorite = favoriteProduct != null,
                favoriteCount
            });
        }

       



  





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
