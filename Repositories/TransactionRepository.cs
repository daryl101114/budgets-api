using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class TransactionRepository: ITransactionRepository
    {
        private readonly BudgetsDbContext _context;
        public TransactionRepository(BudgetsDbContext dbContext) {
            _context = dbContext;
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
        {
           return await _context.Transactions.Where(t => t.Id == transactionId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TransactionCategory>> GetTransactionCategoriesAsync()
        {
            return await _context.TransactionCategory.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid walletId)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.TransactionCategory)
                .Where(t => t.WalletId == walletId)
                .ToListAsync();
        }

        public async Task<bool> SaveTransactionAsync()
        {
            //Save entity when 0 or more entities have been saved
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void UpdateTransactionAsync(Transaction transaction)
        {
            _context.Update(transaction);
        }
    }
}
