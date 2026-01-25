
using MechanicShop.Application;
using MechanicShop.Infrastructure;
using MechanicShop.Infrastructure.Data;
using MechanicShop.Infrastructure.RealTime;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MechanicShop.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddPresentation(builder.Configuration);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                await app.InitialiseDatabaseAsync();
            }

            #region Migrate Database

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            }


            #endregion

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<WorkOrderHub>("/hubs/workorders");

            app.Run();
        }
    }
}
