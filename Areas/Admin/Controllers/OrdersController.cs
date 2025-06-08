using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web_1773.Data;
using Web_1773.Models;

namespace Web_1773.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CurrentFilter"] = searchString;

            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.Id.ToString().Contains(searchString) ||
                    (o.User != null && o.User.Email != null && o.User.Email.Contains(searchString)) ||
                    (o.ShippingAddress != null && o.ShippingAddress.Contains(searchString))
                );
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "date_desc":
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
                case "Price":
                    orders = orders.OrderBy(o => o.TotalPrice);
                    break;
                case "price_desc":
                    orders = orders.OrderByDescending(o => o.TotalPrice);
                    break;
                default:
                    orders = orders.OrderBy(o => o.OrderDate);
                    break;
            }

            return View(await orders.ToListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            
            // Xóa các OrderDetails trước
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == id)
                .ToListAsync();
                
            _context.OrderDetails.RemoveRange(orderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = $"Đã xóa đơn hàng #{id} thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}