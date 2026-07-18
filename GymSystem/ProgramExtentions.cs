using GymSystem.DAL.Contexts;
using GymSystem.DAL.DataSeeds;
using Microsoft.EntityFrameworkCore;

namespace GymSystem
{
    public static class ProgramExtentions
    {
        public static async Task MigrateAndSeedAsync(this WebApplication app)
        {
            // GymDbContext dbContext = new GymDbContext();//Options
            //Migration + Seed => DbContext Object

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var Configurations = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            //Pending Migrations ??
            var Pending = await dbContext.Database.GetPendingMigrationsAsync();

            if (Pending.Any())
            {
                logger.LogInformation($"Apply {Pending.Count()} Pending Migrations ... ");
                await dbContext.Database.MigrateAsync(); // Apply Update Database
            }
            //Seed
            var SeedPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");

            await GymDataSeed.SeedAsync(dbContext, SeedPath, logger);
        }

        


    }

}
