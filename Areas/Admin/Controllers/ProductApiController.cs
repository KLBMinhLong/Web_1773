using Microsoft.AspNetCore.Mvc;
using Web_1773.Models;
using Web_1773.Repositories;
using System.Linq;

namespace Web_1773.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductApiController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category != null ? p.Category.Name : null
            });
            return Ok(productDtos);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var p = await _productRepository.GetByIdAsync(id);
            if (p == null)
            {
                return NotFound();
            }
            var productDto = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category != null ? p.Category.Name : null
            };
            return Ok(productDto);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] Product product, [FromForm] IFormFile? mainImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Xử lý lưu ảnh nếu có
            if (mainImage != null && mainImage.Length > 0)
            {
                var fileName = Path.GetFileName(mainImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mainImage.CopyToAsync(stream);
                }

                product.ImageUrl = "/images/" + fileName;
            }

            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] Product product, [FromForm] IFormFile? ImageFile)
        {
            try
            {
                // Ensure ID matches
                if (product != null)
                {
                    product.Id = id;
                }

                // Basic validation
                if (product == null)
                {
                    return BadRequest(new { error = "Product data is required" });
                }

                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    return BadRequest(new { errors = new { Name = new[] { "Tên sản phẩm là bắt buộc" } } });
                }

                if (product.Price <= 0)
                {
                    return BadRequest(new { errors = new { Price = new[] { "Giá sản phẩm phải lớn hơn 0" } } });
                }

                if (product.CategoryId <= 0)
                {
                    return BadRequest(new { errors = new { CategoryId = new[] { "Vui lòng chọn danh mục" } } });
                }

                // Lấy sản phẩm hiện tại từ DB để giữ lại ImageUrl nếu không có ảnh mới
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new { error = $"Product with ID {id} not found" });
                }

                // Giữ lại ImageUrl cũ nếu không có ảnh mới
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                // Xử lý lưu ảnh mới nếu có
                else
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Ensure images directory exists
                    var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    product.ImageUrl = "/images/" + fileName;
                }

                await _productRepository.UpdateAsync(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Failed to update product: {ex.Message}" });
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { error = $"Product with ID {id} not found" });
                }

                await _productRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Failed to delete product: {ex.Message}" });
            }
        }
    }
}
