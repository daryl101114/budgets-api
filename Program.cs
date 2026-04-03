
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
using System.Text.Json.Serialization;

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
            var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"]
                ?? throw new InvalidOperationException("Cors:AllowedOrigin is not configured.");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigin)
                              .AllowCredentials()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            //Logger Congfiguration
            builder.Host.UseSerilog();

            //Configure Controllers
            builder.Services.AddControllers(opt =>
            {
                opt.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

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

            //Adds Service and Repositories Implementations to Dependency Injection
            builder.Services.ConfigureServices();

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
            app.UseCors("AllowSpecificOrigin");

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
