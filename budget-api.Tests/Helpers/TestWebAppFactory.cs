using budget_api.DbConext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace budget_api.Tests.Helpers
{
    public class TestWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove ALL EF Core DbContext registrations to prevent SQL Server being used
                var toRemove = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<BudgetsDbContext>) ||
                        d.ServiceType == typeof(DbContextOptions) ||
                        d.ServiceType == typeof(BudgetsDbContext))
                    .ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                // Replace with isolated InMemory DB per factory instance
                services.AddDbContext<BudgetsDbContext>(options =>
                    options.UseInMemoryDatabase(_dbName));
            });
        }
    }
}
