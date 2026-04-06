using budget_api.Models.DTOs;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/wallets/{walletId}/import-profile")]
    public class WalletImportProfileController : ControllerBase
    {
        private readonly IWalletImportProfileService _profileService;

        public WalletImportProfileController(IWalletImportProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid walletId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var profile = await _profileService.GetByWalletIdAsync(walletId, userId.Value);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> Upsert(Guid walletId, [FromBody] UpsertWalletImportProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var profile = await _profileService.UpsertAsync(walletId, userId.Value, dto);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid walletId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _profileService.DeleteAsync(walletId, userId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private Guid? GetUserId()
        {
            var value = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            return value == null ? null : new Guid(value);
        }
    }
}
