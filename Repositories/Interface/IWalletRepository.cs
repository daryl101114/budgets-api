using budget_api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace budget_api.Repositories.Interface
{
    public interface IWalletRepository
    {
        Task CreateWalletAsync(Wallets wallets);
        Task<bool> SaveChangesAsync();
    }
}
