using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Shop_Classix.Helper;
using Microsoft.AspNetCore.Authorization;
using Shop_Classix.Models;
using System.Security.Claims;
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


        [HttpGet]
        [Authorize]
        public IActionResult CheckOut()
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();


            // Create the checkout model
            var model = new CheckOutViewModel
            {
                Items = cart.Items,
                Total = cart.Items.Sum(item => item.TotalPrice)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CheckOut(CheckOutViewModel model)
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            if (cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Cart", "Cart");
            }

            //lấy thông tin cookie email
            var customerEmail = HttpContext.User.Claims.SingleOrDefault(p => p.Type == ClaimTypes.Email)?.Value;

            //kiểm tra email khách hàng có giống với cookie email đã đăng nhập
            var customer = dataContext.customers.SingleOrDefault(c => c.Email == customerEmail);

            if (customer == null)
            {
                ModelState.AddModelError("", "Customer not found");
                return View(model);
            }

            //tạo đơn hàng
            var order = new OrderModel
            {
                TotalPrice = cart.Items.Sum(item => item.TotalPrice),
                Status = 1,
                PaymentMethod = model.PaymentMethod,
                CustomerId = customer.Id,
                CustomerName = model.Receiver,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone,
                OrderNotes = model.OrderNotes,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now
            };

            try
            {
                dataContext.orders.Add(order);
                dataContext.SaveChanges();

                //lưu đơn hàng chi tiết
                var orderDetails = cart.Items.Select(item => new OrderDetailModel
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList();

                dataContext.orderDetails.AddRange(orderDetails);
                dataContext.SaveChanges();

                // xóa giỏ hàng
                HttpContext.Session.Remove("Cart");

                TempData["ThankYouMessage"] = "Thank you for your order! We will process it shortly.";

                return RedirectToAction("CheckOut","Cart");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during checkout: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred during checkout. Please try again.";
                return View(model);
            }
        }





    }
}
