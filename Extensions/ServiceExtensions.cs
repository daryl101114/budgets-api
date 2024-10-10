using budget_api.Repositories;
using budget_api.Repositories.Interface;
using budget_api.Services;
using budget_api.Services.Interface;
using System.Runtime.CompilerServices;

namespace budget_api.Extensions
{
    public static class ServiceExtensions
    {

        public static void ConfigureServices(this IServiceCollection services)
        {
            //Register Services
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IWalletService, WalletService>();
            //Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
        }
    }
}
