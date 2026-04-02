using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [Route("api/wallets")]
    [Authorize]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private readonly IWalletService _walletService;

        public WalletController(ILogger<WalletController> logger, IWalletService walletService)
        {
            _logger = logger;
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserWallets()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                var userWallets = await _walletService.GetUserWalletsAsync(new Guid(userId));
                if (userWallets == null) return NoContent();

                return Ok(userWallets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve wallets");
                return StatusCode(500, "A problem occurred while retrieving wallets.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet(CreateWalletDto createWalletDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _walletService.CreateWalletAsync(createWalletDto, new Guid(userId));
                return StatusCode(201, "Wallet created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create wallet");
                return StatusCode(500, "A problem occurred while handling request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWallet(Guid id, UpdateWalletDto updateWalletDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _walletService.UpdateWalletAsync(id, new Guid(userId), updateWalletDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update wallet {WalletId}", id);
                return StatusCode(500, "A problem occurred while handling request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(Guid id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (userId == null) return Unauthorized();

                await _walletService.DeleteWalletAsync(id, new Guid(userId));
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete wallet {WalletId}", id);
                return StatusCode(500, "A problem occurred while handling request.");
            }
        }
    }
}
