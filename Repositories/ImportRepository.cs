using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class ImportRepository : IImportRepository
    {
        private readonly BudgetsDbContext _context;

        public ImportRepository(BudgetsDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(ImportBatch batch)
        {
            await _context.ImportBatches.AddAsync(batch);
        }

        public async Task<ImportBatch?> GetByIdAsync(Guid batchId, Guid userId)
        {
            return await _context.ImportBatches
                .FirstOrDefaultAsync(b => b.Id == batchId && b.UserId == userId);
        }

        public async Task<IEnumerable<ImportBatch>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ImportBatches
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetBatchTransactionsAsync(Guid batchId)
        {
            return await _context.Transactions
                .Where(t => t.ImportId == batchId && t.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<HashSet<string>> GetExistingHashesAsync(Guid walletId)
        {
            var hashes = await _context.Transactions
                .Where(t => t.WalletId == walletId && t.DeletedAt == null && t.DuplicateHash != null)
                .Select(t => t.DuplicateHash!)
                .ToListAsync();
            return hashes.ToHashSet();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
