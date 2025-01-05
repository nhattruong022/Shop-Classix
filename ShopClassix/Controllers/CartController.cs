using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Shop_Classix.Helper;
using Microsoft.AspNetCore.Authorization;
namespace Shop_Classix.Controllers
{
    public class CartController : Controller
    {
      
        private readonly DataContext dataContext;
        private const int PageSize = 1;

        public CartController(DataContext context)
        {
            dataContext = context;
        }

        // Hiển thị giỏ hàng
        public async Task<IActionResult> Cart(int page = 1)
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            var totalOrders = cart.Items.Count;
            var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

            cart.Items = cart.Items
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart");
            if (cart != null)
            {
                var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == id);
                if (existingItem != null)
                {
                    existingItem.Quantity = quantity; // Cập nhật số lượng
                }
            }

            HttpContext.Session.Set("Cart", cart);
            return Ok(); // Trả về phản hồi thành công
        }


        [HttpPost] // Đảm bảo rằng đây là yêu cầu POST


        public IActionResult AddToCart(int id, int quantity = 1) // Thêm tham số quantity

        {
            var product = dataContext.products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Tăng số lượng theo giá trị từ input
            }
            else
            {
                var cartItem = new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity, // Sử dụng giá trị từ input
                    ImageUrl = product.Image
                };
                cart.Items.Add(cartItem);
            }

            HttpContext.Session.Set("Cart", cart);

            // Trả về phản hồi JSON thay vì chuyển hướng
            return Json(new { success = true, message = "Thêm vào giỏ thành công!" });
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart");
            if (cart != null)
            {
                var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == productId);
                if (itemToRemove != null)
                {
                    cart.Items.Remove(itemToRemove); // Xóa sản phẩm khỏi giỏ hàng
                }
            }

            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Cart"); // Chuyển hướng về trang giỏ hàng
        }


        [HttpPost]
        public IActionResult RemoveAllFromCart()
        {
            // Xóa giỏ hàng bằng cách thiết lập lại session
            HttpContext.Session.Set("Cart", new CartViewModel());
            return RedirectToAction("Cart"); // Chuyển hướng về trang giỏ hàng
        }


        [Authorize]
        public IActionResult CheckOut()
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();
            return View(cart.Items);
        }




    }
}
