using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_1773.Data;
using Web_1773.Helpers;
using Web_1773.Models;
using Web_1773.Repositories;

namespace Web_1773.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartController(
            IProductRepository productRepository, 
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            
            if (product == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.ImageUrl
            };

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            TempData["SuccessMessage"] = $"{product.Name} đã được thêm vào giỏ hàng";
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            
            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            TempData["SuccessMessage"] = "Sản phẩm đã được xóa khỏi giỏ hàng";
            
            return RedirectToAction("Index");
        }

        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            
            if (cart != null)
            {
                if (quantity <= 0)
                {
                    cart.RemoveItem(productId);
                }
                else
                {
                    cart.UpdateQuantity(productId, quantity);
                }
                
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            TempData["SuccessMessage"] = "Giỏ hàng đã được xóa";
            
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            
            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index");
            }
            
            // Tạo order với dữ liệu tạm
            var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalAmount,
                // Các trường OrderDetails sẽ được xử lý trong form hidden
            };
            
            return View(order);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                
                if (cart == null || !cart.Items.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                    return RedirectToAction("Index");
                }

                // Kiểm tra địa chỉ giao hàng
                if (string.IsNullOrWhiteSpace(order.ShippingAddress))
                {
                    ModelState.AddModelError("ShippingAddress", "Vui lòng nhập địa chỉ giao hàng");
                    return View(order);
                }

                // Nếu mô hình OrderDetails là null do không bind được từ form
                if (order.OrderDetails == null || !order.OrderDetails.Any())
                {
                    // Xây dựng lại từ session
                    order.OrderDetails = cart.Items.Select(i => new OrderDetail
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList();
                }

                // Xác thực UserId
                if (string.IsNullOrEmpty(order.UserId))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "Không thể xác thực người dùng, vui lòng đăng nhập lại";
                        return RedirectToAction("Login", "Account", new { area = "Identity" });
                    }
                    order.UserId = user.Id;
                }
                
                // Đảm bảo OrderDate được thiết lập
                if (order.OrderDate == DateTime.MinValue)
                {
                    order.OrderDate = DateTime.Now;
                }
                
                // Đảm bảo TotalPrice được thiết lập chính xác
                if (order.TotalPrice <= 0)
                {
                    order.TotalPrice = cart.TotalAmount;
                }

                // Log thông tin đơn hàng để debug
                Console.WriteLine($"Saving order: UserId={order.UserId}, Address={order.ShippingAddress}, Items={order.OrderDetails?.Count ?? 0}");

                _context.Orders.Add(order);
                var result = await _context.SaveChangesAsync();
                
                Console.WriteLine($"SaveChanges result: {result}");

                if (result > 0)
                {
                    HttpContext.Session.Remove("Cart");
                    TempData["SuccessMessage"] = "Đơn hàng của bạn đã được đặt thành công!";
                    return RedirectToAction("OrderCompleted", new { id = order.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Không thể lưu đơn hàng. Vui lòng thử lại sau.");
                    return View(order);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                return View(order);
            }
        }

        public async Task<IActionResult> OrderCompleted(int id)
        {
            var order = await _context.Orders
        .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
        .FirstOrDefaultAsync(o => o.Id == id);
                
            if (order == null)
            {
                return NotFound();
            }
            
            return View(order);
        }
    }
}