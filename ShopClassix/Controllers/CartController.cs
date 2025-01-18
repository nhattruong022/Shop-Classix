using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Shop_Classix.Helper;
using Microsoft.AspNetCore.Authorization;
using Shop_Classix.Models;
using System.Security.Claims;
using Shop_Classix.Service;
using System.Net.WebSockets;
using Shop_Classix.Models.VnPay;
using System.Text.RegularExpressions;
namespace Shop_Classix.Controllers
{

    public class CartController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly DataContext dataContext;
        private const int PageSize = 4;

        public CartController(DataContext context, IVnPayService vnPayService)
        {
            dataContext = context;
            _vnPayService = vnPayService;
        }
        private int GetUniqueProductCount()
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();
            return cart.Items.Select(item => item.ProductId).Distinct().Count();
        }


        //[Authorize]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> Cart(int page = 1)
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            ViewBag.UniqueProductCount = GetUniqueProductCount();

            var totalOrders = cart.Items.Count;
            var totalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);
            var paginatedItems = cart.Items
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            if (totalOrders > 0)
            {
                ViewBag.TotalAmount = cart.TotalAmount;
            }
            else
            {
                ViewBag.TotalAmount = 0;
            }
            /* ViewBag.TotalAmount = cart.TotalAmount;*/ // Sử dụng TotalAmount từ CartViewModel

            //ViewBag.TotalAmount = CalculateTotalAmount(cart.Items);
            //ViewBag.CurrentPageTotalAmount = CalculateTotalAmount(paginatedItems); 



            // Trả về giỏ hàng với các sản phẩm đã phân trang
            cart.Items = paginatedItems;

            cart.Items = paginatedItems;

            // Trả về giỏ hàng với các sản phẩm đã phân trang
            cart.Items = paginatedItems;


            return View(cart);
        }



        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart");
            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == id);

            // Kiểm tra nếu sản phẩm không tồn tại trong giỏ hàng
            if (existingItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng." });
            }

            // Lấy tổng số lượng sản phẩm đã có trong các đơn hàng
            int totalQuantityInOrders = dataContext.orderDetails
                .Where(od => od.ProductId == id)
                .Sum(od => od.Quantity);
            
            // Lấy sản phẩm từ bảng products
            var product = dataContext.products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }
            int productQuantity = product.Quantity.GetValueOrDefault();
            int availableQuantity = productQuantity - totalQuantityInOrders;
         
            // Kiểm tra số lượng có thể cập nhật
            if (quantity > availableQuantity)
            {
                if (availableQuantity <= 0)
                {
                    return Json(new { success = false, message = "Sản phẩm đã hết hàng." });
                }
                else
                {
                    return Json(new { success = false, message = $"Không thể cập nhật số lượng. Số lượng tối đa có thể cập nhật là {availableQuantity}." });
                }
            }

            // Cập nhật số lượng trong giỏ hàng
            existingItem.Quantity = quantity;
            HttpContext.Session.Set("Cart", cart);

            return Json(new { success = true, message = "Cập nhật số lượng thành công." });
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            if (quantity < 1)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn hoặc bằng 1." });
            }

            var product = dataContext.products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Lấy tổng số lượng sản phẩm đã có trong giỏ hàng
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();
            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == product.Id);
            int totalQuantityInOrders = dataContext.orderDetails
                .Where(od => od.ProductId == id)
                .Sum(od => od.Quantity);
            int cartQuantity = cart.Items
        .Where(item => item.ProductId == id)
        .Sum(item => item.Quantity);
            // Kiểm tra số lượng có thể thêm vào giỏ hàng
            int productQuantity = product.Quantity.GetValueOrDefault();
            int availableQuantity = productQuantity - totalQuantityInOrders- cartQuantity;
            if (quantity > availableQuantity)
            {
                if (availableQuantity <= 0)
                {
                    return Json(new { success = false, message = "Sản phẩm đã hết hàng." });
                }
                else
                {
                    return Json(new { success = false, message = $"Không thể thêm sản phẩm này vào giỏ hàng. Số lượng tối đa có thể thêm là {availableQuantity}." });
                }
            }

            // Thêm sản phẩm vào giỏ hàng
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.Image
                };
                cart.Items.Add(cartItem);
            }

            HttpContext.Session.Set("Cart", cart);
            return Json(new { success = true, message = $"Thêm vào giỏ hàng thành công!" });
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart");
            if (cart != null)
            {
                var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == id);
                if (itemToRemove != null)
                {
                    cart.Items.Remove(itemToRemove);
                }
            }

            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Cart", new { page = 1 });
        }


        [HttpPost]
        public IActionResult RemoveAllFromCart()
        {
            HttpContext.Session.Set("Cart", new CartViewModel());
            return RedirectToAction("Cart");
        }


        [HttpGet]
        [Authorize]
        public IActionResult CheckOut()
        {
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();


            // Tổng giá trị giỏ hàng
            var totalPrice = cart.Items.Sum(item => item.TotalPrice);

            // Tính tiền cọc (10%)
            var depositAmount = totalPrice * 0.1;


            // Create the checkout model
            var model = new CheckOutViewModel
            {
                Items = cart.Items,
                Total = cart.Items.Sum(item => item.TotalPrice),
                deposit = depositAmount
            };



            return View(model);
        }


        [HttpPost]
        [Authorize]
        public IActionResult CheckOut(CheckOutViewModel model, string payment = "C0D")
        {
            // Thiết lập giỏ hàng
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            // Kiểm tra nếu giỏ hàng trống
            if (cart.Items == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items to your cart before checking out.";
                return RedirectToAction("Index", "Cart"); // Chuyển hướng về trang giỏ hàng
            }

            // Tính tiền cọc 10%
            var totalPrice = cart.Items.Sum(item => item.TotalPrice);
            var depositAmount = totalPrice * 0.1;

            model.Items = cart.Items;
            model.Total = totalPrice;
            model.deposit = depositAmount;

            // Kiểm tra email có lưu trong cookie chưa
            var customerEmail = HttpContext.User.Claims.SingleOrDefault(p => p.Type == ClaimTypes.Email)?.Value;

            // Kiểm tra email có trong database không
            var customer = dataContext.customers.SingleOrDefault(c => c.Email == customerEmail);

            // Tạo đơn hàng
            var order = new OrderModel
            {
                TotalPrice = totalPrice,
                deposit = depositAmount,
                Status = 1,
                PaymentMethod = payment == "Payment VNPAY" ? "VnPay" : "COD",
                CustomerId = customer.Id,
                CustomerName = model.Receiver,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone,
                OrderNotes = string.IsNullOrEmpty(model.OrderNotes) ? null : model.OrderNotes,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now
            };

            try
            {
                dataContext.orders.Add(order);
                dataContext.SaveChanges();

                // Lưu đơn hàng chi tiết
                var orderDetails = cart.Items.Select(item => new OrderDetailModel
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList();

                dataContext.orderDetails.AddRange(orderDetails);
                dataContext.SaveChanges();

                if (payment == "Payment VNPAY")
                {
                    // Lưu đơn hàng VNPAY
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = totalPrice,
                        CreatedDate = DateTime.Now,
                        description = $"{model.Receiver} {model.Phone}",
                        FullName = model.Receiver,
                        OrderId = order.Id.ToString()
                    };

                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                // Xóa giỏ hàng sau khi lưu đơn hàng
                HttpContext.Session.Remove("Cart");

                // Hiện thông báo cảm ơn
                TempData["ThankYouMessage"] = "Thank you for your order! We will process it shortly.";
                return RedirectToAction("CheckOut", "Cart");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during checkout: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred during checkout. Please try again.";
                return View(model);
            }
        }



        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }



        //sau khi thanh toán xong thì trả về gì
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.VnPayResponseCode == "00") // Transaction successful
            {
                var orderId = int.Parse(response.OrderId);

                // Retrieve the order
                var order = await dataContext.orders.FindAsync(orderId);

                if (order != null)
                {
                    //Lưu đơn hàng VnPAY
                    var vnPayModel = new VnPayModel
                    {
                        OrderId = response.OrderId,
                        PaymentMethod = response.PaymentMethod,
                        description = response.OrderDescription,
                        TransactionId = response.TransactionId,
                        PaymentId = response.PaymentId,
                        createdAt = DateTime.Now
                    };

                    dataContext.Add(vnPayModel);

                    // CẬP NHẬP TRẠNG THÁI PAYMENT
                    order.Status = 2; // status =2 -> payment vnpay
                    dataContext.orders.Update(order);

                    await dataContext.SaveChangesAsync();

                    //xóa giỏ hàng sau khi lưu đơn hàng
                    HttpContext.Session.Remove("Cart");

                    TempData["ThankYouMessage"] = "Payment successful! Your order has been placed.";
                    return View(response);
                }
            }

            TempData["ErrorMessage"] = "Payment failed or invalid response.";
            return RedirectToAction("PaymentFail", "Cart");
        }





    }
}
