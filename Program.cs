
using budget_api.DbConext;
using Microsoft.EntityFrameworkCore;
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

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Configure DB Connections
            builder.Services.AddDbContext<BudgetsDbContext>(
                dbContextOptions =>
                {
                    dbContextOptions.UseSqlServer(builder.Configuration["ConnectionStrings:BudgetsInfoDbConnection"]);
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
