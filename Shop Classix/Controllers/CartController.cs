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

        public CartController(DataContext context)
        {
            dataContext = context;
        }

        // Hiển thị giỏ hàng
        public IActionResult Cart()
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();
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

        public IActionResult AddToCart(int id, int quantity=1) // Thêm tham số quantity
        {
            var product = dataContext.products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            var existingItem = cart.Items.FirstOrDefault(item => item.ProductName == product.Name);
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

            return RedirectToAction("Cart"); // Chuyển hướng đến trang giỏ hàng
        }


        [Authorize]
        public IActionResult CheckOut()
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();
            return View(cart.Items);
        }




    }
}
