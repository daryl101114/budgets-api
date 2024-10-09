using budget_api.Repositories.Interface;
using budget_api.Services;
using budget_api.Services.Interface;
using System.Runtime.CompilerServices;

namespace budget_api.Extensions
{
    public static class ServiceExtensions
    {
        //public readonly IServiceCollection _services;
        //public ServiceExtensions( IServiceCollection services)
        //{
        //    _services = services;
        //}

        public static void ConfigureServices(this IServiceCollection services)
        {
            //Register Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            //Register Repositories
            services.AddScoped<IUserRepository, IUserRepository>();
        }
    }
}
