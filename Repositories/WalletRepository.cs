using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly BudgetsDbContext _context;

        public WalletRepository(BudgetsDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task CreateWalletAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
        }

        public async Task<ICollection<Wallet>> GetWalletsByUserIdAsync(Guid userId)
        {
            return await _context.Wallets
                .Where(w => w.UserId == userId && w.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Wallet?> GetWalletByIdAsync(Guid walletId, Guid userId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId && w.DeletedAt == null);
        }

        public Task UpdateWalletAsync(Wallet wallet)
        {
            wallet.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task SoftDeleteWalletAsync(Wallet wallet)
        {
            wallet.DeletedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
