using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Web_1773.Models;
using Web_1773.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Web_1773.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách sản phẩm 
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }


        // Hiển thị form thêm sản phẩm mới 
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }        // Xử lý thêm sản phẩm mới 
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile mainImage, List<IFormFile> additionalImages)
        {
            if (ModelState.IsValid)
            {
                // Xử lý hình ảnh chính
                if (mainImage != null && mainImage.Length > 0)
                {
                    product.ImageUrl = await SaveImage(mainImage);
                }

                // Xử lý các hình ảnh phụ
                if (additionalImages != null && additionalImages.Count > 0)
                {
                    product.ImageUrls = new List<string>();
                    foreach (var file in additionalImages)
                    {
                        if (file.Length > 0)
                        {
                            var imagePath = await SaveImage(file);
                            product.ImageUrls.Add(imagePath);
                        }
                    }
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập 
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hiển thị thông tin chi tiết sản phẩm 
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm 
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm 
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productRepository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm 
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }        // Xử lý xóa sản phẩm 
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }        // Phương thức lưu hình ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            // Tạo tên file duy nhất để tránh trùng lặp
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine("wwwroot/images", fileName);

            // Tạo thư mục nếu chưa tồn tại
            var directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + fileName; // Trả về đường dẫn tương đối
        }
    }
}
