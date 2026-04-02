using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto createTransactionDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _transactionService.CreateTransactionAsync(createTransactionDto, new Guid(userId));
                return StatusCode(201, "Transaction created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create transaction");
                return StatusCode(500, "Failed to create transaction.");
            }
        }

        [HttpGet("wallet/{walletId}")]
        public async Task<IActionResult> GetWalletTransactions(Guid walletId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                var transactions = await _transactionService.GetTransactionsByWalletAsync(walletId, new Guid(userId));
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve transactions for wallet {WalletId}", walletId);
                return StatusCode(500, "Failed to get transactions.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                var transaction = await _transactionService.GetTransactionByIdAsync(id, new Guid(userId));
                if (transaction == null) return NotFound();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve transaction {TransactionId}", id);
                return StatusCode(500, "Failed to get transaction.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, UpdateTransactionDto updateTransactionDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _transactionService.UpdateTransactionAsync(id, new Guid(userId), updateTransactionDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update transaction {TransactionId}", id);
                return StatusCode(500, "Failed to update transaction.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _transactionService.DeleteTransactionAsync(id, new Guid(userId));
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete transaction {TransactionId}", id);
                return StatusCode(500, "Failed to delete transaction.");
            }
        }

    }
}
