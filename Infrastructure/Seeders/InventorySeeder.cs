using System.Text.Json;
using InventoryService.Infrastructure.Databases;
using InventoryService.Infrastructure.Helpers;
using InventoryService.Models;

namespace InventoryService.Infrastructure.Seeders
{
    public class InventorySeeder: ISeeder
    {
        public async Task Seed(IamDBContext dbContext, ILogger logger)
        {
            logger.LogInformation("Seeding Inventory...");
            var jsonPath = "SeedersData/Inventory.json";

            var jsonString = await File.ReadAllTextAsync(jsonPath);

            var permissions = JsonSerializer.Deserialize<List<Inventory>>(jsonString, JsonSerializeSeeder.options);
            var newInventories = new List<Inventory>();

            try
            {
                await dbContext.Database.BeginTransactionAsync();

                foreach (var permission in permissions)
                {
                    newInventories.Add(permission);
                }
                await dbContext.Inventories.AddRangeAsync(newInventories);
                await dbContext.SaveChangesAsync();
                await dbContext.Database.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while seeding Inventories");
                await dbContext.Database.RollbackTransactionAsync();
            }

            logger.LogInformation("Seeding Inventories complete");
        }
    }
}