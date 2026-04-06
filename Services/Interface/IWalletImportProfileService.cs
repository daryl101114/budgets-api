using budget_api.Models.DTOs;

namespace budget_api.Services.Interface
{
    public interface IWalletImportProfileService
    {
        Task<WalletImportProfileDto?> GetByWalletIdAsync(Guid walletId, Guid userId);
        Task<WalletImportProfileDto> UpsertAsync(Guid walletId, Guid userId, UpsertWalletImportProfileDto dto);
        Task DeleteAsync(Guid walletId, Guid userId);
    }
}
