using budget_api.Models.DTOs;

namespace budget_api.Services.Interface
{
    public interface IWalletService
    {
        Task CreateWalletAsync(CreateWalletDto createWalletDto, Guid userId);
        Task<UserWalletsDto?> GetUserWalletsAsync(Guid userId);
        Task UpdateWalletAsync(Guid walletId, Guid userId, UpdateWalletDto updateWalletDto);
        Task DeleteWalletAsync(Guid walletId, Guid userId);
    }
}
