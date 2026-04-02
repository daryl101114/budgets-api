using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/import")]
    public class ImportController : ControllerBase
    {
        private readonly ILogger<ImportController> _logger;
        private readonly IImportService _importService;

        public ImportController(ILogger<ImportController> logger, IImportService importService)
        {
            _logger = logger;
            _importService = importService;
        }

        [HttpPost("wallets/{walletId}")]
        public async Task<IActionResult> Upload(Guid walletId, IFormFile file)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                if (file == null || file.Length == 0)
                    return BadRequest("No file provided.");

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Only CSV files are supported.");

                var batch = await _importService.UploadAsync(walletId, userId.Value, file);
                return StatusCode(201, batch);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload import for wallet {WalletId}", walletId);
                return StatusCode(500, "Failed to process import.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBatches()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                return Ok(await _importService.GetBatchesAsync(userId.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get import batches");
                return StatusCode(500, "Failed to get import batches.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBatchDetail(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                var batch = await _importService.GetBatchDetailAsync(id, userId.Value);
                if (batch == null) return NotFound();
                return Ok(batch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get import batch {BatchId}", id);
                return StatusCode(500, "Failed to get import batch.");
            }
        }

        [HttpPatch("{id}/transactions/approve")]
        public async Task<IActionResult> BulkApprove(Guid id, [FromBody] BulkReviewDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _importService.BulkApproveAsync(id, userId.Value, dto.TransactionIds);
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
                _logger.LogError(ex, "Failed to approve transactions for batch {BatchId}", id);
                return StatusCode(500, "Failed to approve transactions.");
            }
        }

        [HttpPatch("{id}/transactions/reject")]
        public async Task<IActionResult> BulkReject(Guid id, [FromBody] BulkReviewDto dto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _importService.BulkRejectAsync(id, userId.Value, dto.TransactionIds);
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
                _logger.LogError(ex, "Failed to reject transactions for batch {BatchId}", id);
                return StatusCode(500, "Failed to reject transactions.");
            }
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _importService.ConfirmAsync(id, userId.Value);
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
                _logger.LogError(ex, "Failed to confirm import batch {BatchId}", id);
                return StatusCode(500, "Failed to confirm import batch.");
            }
        }

        [HttpPost("{id}/rollback")]
        public async Task<IActionResult> Rollback(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized();

                await _importService.RollbackAsync(id, userId.Value);
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
                _logger.LogError(ex, "Failed to rollback import batch {BatchId}", id);
                return StatusCode(500, "Failed to rollback import batch.");
            }
        }

        private Guid? GetUserId()
        {
            var value = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            return value == null ? null : new Guid(value);
        }
    }
}
