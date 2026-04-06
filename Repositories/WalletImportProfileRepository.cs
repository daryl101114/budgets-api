using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class WalletImportProfileRepository : IWalletImportProfileRepository
    {
        private readonly BudgetsDbContext _context;

        public WalletImportProfileRepository(BudgetsDbContext context)
        {
            _context = context;
        }

        public async Task<WalletImportProfile?> GetByWalletIdAsync(Guid walletId)
        {
            return await _context.WalletImportProfiles
                .FirstOrDefaultAsync(p => p.WalletId == walletId);
        }

        public async Task AddAsync(WalletImportProfile profile)
        {
            await _context.WalletImportProfiles.AddAsync(profile);
        }

        public void Delete(WalletImportProfile profile)
        {
            _context.WalletImportProfiles.Remove(profile);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
