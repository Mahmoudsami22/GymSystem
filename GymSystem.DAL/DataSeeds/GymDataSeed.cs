using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GymSystem.DAL.DataSeeds
{
    public static class GymDataSeed
    {
        public static async Task SeedAsync(GymDbContext dbContext, string seedFilesPath, ILogger logger, CancellationToken ct = default)
        {

            try
            {
                if (!await dbContext.Plans.AnyAsync(ct))
                {
                    var plans = LoadDataFromJsonFile<Plan>("plans.json", seedFilesPath);

                    if (plans.Count > 0)
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Seeded {plans.Count} plans.");
                    }
                }
                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gym Data Seeding Failed");
                throw;
            }
        }
        private static List<T> LoadDataFromJsonFile<T>(string fileName, string FolderPath)
        {

            var filePath = Path.Combine(FolderPath, fileName); // FolderPath/FileName

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed Data file not Found: {filePath}");

            var Data = File.ReadAllText(filePath);

            var Options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            Options.Converters.Add(new JsonStringEnumConverter());

            return JsonSerializer.Deserialize<List<T>>(Data, Options) ?? [];
        }
    }
}
    
