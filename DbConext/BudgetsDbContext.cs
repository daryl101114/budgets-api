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
    }
}
