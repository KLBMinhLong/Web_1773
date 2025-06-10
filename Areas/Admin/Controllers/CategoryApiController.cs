using Microsoft.AspNetCore.Mvc;
using Web_1773.Repositories;
using Web_1773.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Web_1773.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryApiController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryApiController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var result = categories.Select(c => new { id = c.Id, name = c.Name });
            return Ok(result);
        }
    }
}
