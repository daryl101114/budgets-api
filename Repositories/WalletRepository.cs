using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly BudgetsDbContext _context;
        public WalletRepository(BudgetsDbContext dbContext ) 
        {
            _context = dbContext;
        }

        public async Task CreateWalletAsync(Wallets wallets)
        {
            await _context.Wallets.AddAsync(wallets);
        }

        public async Task<bool> SaveChangesAsync()
        {
            //Save entity when 0 or more entities have been saved
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
