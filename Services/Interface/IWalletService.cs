using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Services.Interface
{
    public interface IWalletService
    {
        Task CreateWallet(WalletCreationDto walletCreationDto, Guid userId);
        Task<UserWalletsDto?> GetUserWalletsAsync(Guid userId);
    }
}
