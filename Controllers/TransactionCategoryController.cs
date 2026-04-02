using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/transaction-categories")]
    public class TransactionCategoryController : ControllerBase
    {
        private readonly ILogger<TransactionCategoryController> _logger;
        private readonly ITransactionCategoryService _categoryService;

        public TransactionCategoryController(ILogger<TransactionCategoryController> logger, ITransactionCategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                return Ok(await _categoryService.GetCategoriesAsync(new Guid(userId)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get transaction categories");
                return StatusCode(500, "Failed to get transaction categories.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                var category = await _categoryService.GetCategoryByIdAsync(id, new Guid(userId));
                if (category == null) return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get transaction category {CategoryId}", id);
                return StatusCode(500, "Failed to get transaction category.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateTransactionCategoryDto dto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _categoryService.CreateCategoryAsync(dto, new Guid(userId));
                return StatusCode(201, "Category created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create transaction category");
                return StatusCode(500, "Failed to create transaction category.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateTransactionCategoryDto dto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _categoryService.UpdateCategoryAsync(id, new Guid(userId), dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update transaction category {CategoryId}", id);
                return StatusCode(500, "Failed to update transaction category.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _categoryService.DeleteCategoryAsync(id, new Guid(userId));
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete transaction category {CategoryId}", id);
                return StatusCode(500, "Failed to delete transaction category.");
            }
        }
    }
}
