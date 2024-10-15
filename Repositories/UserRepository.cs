using budget_api.DbConext;
using budget_api.Models.DTOs;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly BudgetsDbContext _context;
        public UserRepository(BudgetsDbContext budgetsDbContext) {
            _context = budgetsDbContext ?? throw new ArgumentNullException(nameof(budgetsDbContext));
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<User?> GetUserWalletsAsync(Guid userId)
        {
            var userWallets = await _context.Users.Include(u => u.Wallets).ThenInclude(w => w.WalletType)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            return userWallets;
        }

        public async Task AddUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<bool> SaveChangesAsync()
        {
            //Save entity when 0 or more entities have been saved
            return (await _context.SaveChangesAsync() >= 0) ;
        }
    }
}
