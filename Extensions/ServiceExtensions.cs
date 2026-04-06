using budget_api.Models.CSV.Strategies;
using budget_api.Repositories;
using budget_api.Repositories.Interface;
using budget_api.Services;
using budget_api.Services.Interface;

namespace budget_api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            // Register Services
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IWalletService, WalletService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<ITransactionCategoryService, TransactionCategoryService>();
            services.AddTransient<IBudgetService, BudgetService>();
            services.AddTransient<IImportService, ImportService>();
            services.AddTransient<IWalletImportProfileService, WalletImportProfileService>();

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionCategoryRepository, TransactionCategoryRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<IImportRepository, ImportRepository>();
            services.AddScoped<IWalletImportProfileRepository, WalletImportProfileRepository>();

            // Register CSV format strategies (order matters — checked in sequence)
            services.AddSingleton<IImportFormatStrategy, ChaseCheckingStrategy>();
            services.AddSingleton<IImportFormatStrategy, ChaseSapphireStrategy>();
            services.AddSingleton<IImportFormatStrategy, GenericStrategy>();
        }
    }
}
