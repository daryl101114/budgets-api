using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/budgets")]
    public class BudgetController : ControllerBase
    {
        private readonly ILogger<BudgetController> _logger;
        private readonly IBudgetService _budgetService;

        public BudgetController(ILogger<BudgetController> logger, IBudgetService budgetService)
        {
            _logger = logger;
            _budgetService = budgetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();
                return Ok(await _budgetService.GetBudgetsAsync(userId.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get budgets");
                return StatusCode(500, "Failed to get budgets.");
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBudget()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var budget = await _budgetService.GetActiveBudgetAsync(userId.Value);
                if (budget == null) return NotFound();
                return Ok(budget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active budget");
                return StatusCode(500, "Failed to get active budget.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBudget(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var budget = await _budgetService.GetBudgetByIdAsync(id, userId.Value);
                if (budget == null) return NotFound();
                return Ok(budget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get budget {BudgetId}", id);
                return StatusCode(500, "Failed to get budget.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var budget = await _budgetService.CreateBudgetAsync(dto, userId.Value);
                return StatusCode(201, budget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create budget");
                return StatusCode(500, "Failed to create budget.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _budgetService.UpdateBudgetAsync(id, userId.Value, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update budget {BudgetId}", id);
                return StatusCode(500, "Failed to update budget.");
            }
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateBudget(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _budgetService.ActivateBudgetAsync(id, userId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to activate budget {BudgetId}", id);
                return StatusCode(500, "Failed to activate budget.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _budgetService.DeleteBudgetAsync(id, userId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete budget {BudgetId}", id);
                return StatusCode(500, "Failed to delete budget.");
            }
        }

        // ── Allocations ──────────────────────────────────────────────────────────

        [HttpGet("{id}/allocations")]
        public async Task<IActionResult> GetAllocations(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var budget = await _budgetService.GetBudgetByIdAsync(id, userId.Value);
                if (budget == null) return NotFound();
                return Ok(budget.Allocations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get allocations for budget {BudgetId}", id);
                return StatusCode(500, "Failed to get allocations.");
            }
        }

        [HttpPost("{id}/allocations")]
        public async Task<IActionResult> AddAllocation(Guid id, [FromBody] CreateBudgetAllocationDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _budgetService.AddAllocationAsync(id, userId.Value, dto);
                return StatusCode(201, "Allocation added.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add allocation to budget {BudgetId}", id);
                return StatusCode(500, "Failed to add allocation.");
            }
        }

        [HttpPut("{id}/allocations/{allocationId}")]
        public async Task<IActionResult> UpdateAllocation(Guid id, Guid allocationId, [FromBody] UpdateBudgetAllocationDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _budgetService.UpdateAllocationAsync(id, allocationId, userId.Value, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update allocation {AllocationId}", allocationId);
                return StatusCode(500, "Failed to update allocation.");
            }
        }

        [HttpDelete("{id}/allocations/{allocationId}")]
        public async Task<IActionResult> RemoveAllocation(Guid id, Guid allocationId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _budgetService.RemoveAllocationAsync(id, allocationId, userId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove allocation {AllocationId}", allocationId);
                return StatusCode(500, "Failed to remove allocation.");
            }
        }

        private Guid? GetUserId()
        {
            var value = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            return value == null ? null : new Guid(value);
        }
    }
}
