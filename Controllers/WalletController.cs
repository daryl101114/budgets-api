using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [Route("api/[controller]")]
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
                if (userId == null)
                {
                    throw new Exception("User Id is can't be null");
                }

                //Create Service
                var userWallets = await _walletService.GetUserWalletsAsync(new Guid(userId));

                if(userWallets == null)
                {
                    return NoContent();
                }

                return Ok(userWallets);

            }
            catch(Exception ex)
            {
                _logger.LogError("Failed to retreive wallet", ex);
                return StatusCode(500, "A problem occured while retreiving wallets.");
            }
        }

        [HttpPost("createWallet")]
        public async Task<ActionResult> CreateWallet(WalletCreationDto walletCreationDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                if (userId == null) 
                {
                    throw new Exception("User Id is can't be null");
                }

                //Implement service
                await _walletService.CreateWallet(walletCreationDto, new Guid(userId));

                return Ok("Wallet created");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create Wallet", ex);
                return StatusCode(500, "A problem occured while handling request");
            }

        }

    }
}
