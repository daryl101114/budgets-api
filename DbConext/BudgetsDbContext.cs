using budget_api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace budget_api.DbConext
{
    public class BudgetsDbContext :DbContext
    {
        //Inject DBContextOptions
        public BudgetsDbContext(DbContextOptions<BudgetsDbContext> options) : base(options)  { }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallets> Wallets { get; set; }
        public DbSet<WalletType> WalletTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionCategory> TransactionCategory { get; set; }

    }
}
