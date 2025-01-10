using Microsoft.AspNetCore.Mvc;
using Shop_Classix.Repository;
using Shop_Classix.Models.ViewModels;
using Shop_Classix.Helper;
using Microsoft.AspNetCore.Authorization;
using Shop_Classix.Models;
using System.Security.Claims;
using Shop_Classix.Service;
using System.Net.WebSockets;
namespace Shop_Classix.Controllers
{
    public class CartController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly DataContext dataContext;
        private const int PageSize = 2;

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

            return View(cart);
        }

        //private double CalculateTotalAmount(List<CartItemViewModel> items)
        //{
        //    return items.Sum(item => item.TotalPrice);
        //}

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity < 1)
            {
                return BadRequest("Số lượng phải lớn hơn hoặc bằng 1.");
            }

            var cart = HttpContext.Session.Get<CartViewModel>("Cart");
            if (cart != null)
            {
                var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == id);
                if (existingItem != null)
                {
                    existingItem.Quantity = quantity;
                }
            }

            HttpContext.Session.Set("Cart", cart);
            return Ok();
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            if (quantity < 1)
            {
                return BadRequest("Số lượng phải lớn hơn hoặc bằng 1.");
            }

            var product = dataContext.products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == product.Id);
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
            return Json(new { success = true, message = "Thêm vào giỏ thành công!" });

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





            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CheckOut(CheckOutViewModel model, string payment = "C0D")
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<CartViewModel>("Cart") ?? new CartViewModel();

            if (cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Cart", "Cart");
            }


            // Tổng giá trị giỏ hàng và tiền cọc
            var totalPrice = cart.Items.Sum(item => item.TotalPrice);
            var depositAmount = totalPrice * 0.1;

            model.Items = cart.Items;
            model.Total = totalPrice;
            model.deposit = depositAmount;

            if (payment == "Payment VNPAY")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = cart.Items.Sum(item => item.TotalPrice),
                    CreatedDate = DateTime.Now,
                    description = $"{model.Receiver} {model.Phone}",
                    FullName = model.Receiver,
                    OrderId = DateTime.UtcNow.Ticks.ToString()
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
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
                deposit = depositAmount,
                Status = 1,
                PaymentMethod = "COD",
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

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success", "Cart");
        }

        //sau khi thanh toán xong thì trả về gì
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00") //00 là giao dịch thành công
            {
                TempData["Message"] = $"Failed Pay VNPAY:{response.VnPayResponseCode} ";
                return RedirectToAction("PaymentFail");
            }


            TempData["Message"] = $"Success Pay VNPAY";

            return RedirectToAction("PaymentSuccess");
        }




    }
}
