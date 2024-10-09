
using budget_api.DbConext;
using budget_api.Extensions;
using budget_api.Models.Entities;
using budget_api.Repositories;
using budget_api.Repositories.Interface;
using budget_api.Services;
using budget_api.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace budget_api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Configure Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("C:\\Users\\daryl\\OneDrive\\Documents\\Budget-API-Logs\\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            //Logger Congfiguration
            builder.Host.UseSerilog();

            //Configure Controllers
            builder.Services.AddControllers(opt =>
            {
                opt.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters();

            builder.Services.AddProblemDetails();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Configure DB Connections
            builder.Services.AddDbContext<BudgetsDbContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseSqlServer(builder.Configuration["ConnectionStrings:BudgetsInfoDbConnection"]);
                });

            // Register PasswordHasher
            builder.Services.AddScoped<PasswordHasher<User>>();

            //builder.Services.ConfigureServices();
            #region Registered Services & Repositories
            //Register Services
            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

            //Register Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            #endregion

            //Configure AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //Configure API Authentication
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Authentication:Issuer"],
                        ValidAudience = builder.Configuration["Authentication:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Convert.FromBase64String(
                                builder.Configuration["Authentication:SecretForKey"]))
                    };
                });
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
