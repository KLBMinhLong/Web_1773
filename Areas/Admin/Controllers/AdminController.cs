using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web_1773.Data;
using Web_1773.Models;
using Web_1773.Repositories;

namespace Web_1773.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get all products once to avoid multiple database calls
                var allProducts = await _productRepository.GetAllAsync();
                ViewBag.ProductCount = allProducts.Count();
                
                // Get all categories once
                var allCategories = await _categoryRepository.GetAllAsync();
                ViewBag.CategoryCount = allCategories.Count();
                
                // Get user count
                ViewBag.UserCount = await _userManager.Users.CountAsync();
                
                // Get order count
                ViewBag.OrderCount = await _context.Orders.CountAsync();
                
                // Get recent orders (top 5)
                ViewBag.RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync();

                // Get recent products (top 5)
                ViewBag.RecentProducts = allProducts
                    .OrderByDescending(p => p.Id) // Assuming Id is auto-increment and newer products have higher Ids
                    .Take(5)
                    .ToList();

                // Get recent categories (top 5) with product counts
                ViewBag.RecentCategories = allCategories
                    .OrderByDescending(c => c.Id) // Assuming Id is auto-increment
                    .Take(5)
                    .Select(c => new
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ProductCount = allProducts.Count(p => p.CategoryId == c.Id)
                    })
                    .ToList();

                return View();
            }
            catch (System.Exception ex)
            {
                // Log the error
                // You may want to add proper logging here
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
    }
}
