using budget_api.Models.Entities;
using budget_api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace budget_api.DbConext
{
    public class BudgetsDbContext : DbContext
    {
        public BudgetsDbContext(DbContextOptions<BudgetsDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionCategory> TransactionCategory { get; set; }
        public DbSet<SpendingBucket> SpendingBuckets { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetAllocation> BudgetAllocations { get; set; }
        public DbSet<ImportBatch> ImportBatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Wallet — store enums as strings
            modelBuilder.Entity<Wallet>()
                .Property(w => w.WalletType)
                .HasConversion<string>();

            modelBuilder.Entity<Wallet>()
                .Property(w => w.Currency)
                .HasConversion<string>();

            // Transaction — store enums as strings
            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionType)
                .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Source)
                .HasConversion<string>();

            // Transaction indexes
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => new { t.UserId, t.Date });

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.DuplicateHash);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => new { t.WalletId, t.Date });

            // Budget — partial unique index: only one active budget per user
            modelBuilder.Entity<Budget>()
                .HasIndex(b => b.UserId)
                .HasFilter("[IsActive] = 1")
                .IsUnique();

            // ImportBatch — restrict cascade on UserId to avoid multiple cascade paths
            modelBuilder.Entity<ImportBatch>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
