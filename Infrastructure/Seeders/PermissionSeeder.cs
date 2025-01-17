using System.Text.Json;
using InventoryService.Infrastructure.Databases;
using InventoryService.Infrastructure.Helpers;
using InventoryService.Models;

namespace InventoryService.Infrastructure.Seeders
{
  public class PermissionSeeder : ISeeder
  {
    public async Task Seed(IamDBContext dbContext, ILogger logger)
    {
      logger.LogInformation("Seeding Permissions...");
      var jsonPath = "SeedersData/Permission.json";

      var jsonString = await File.ReadAllTextAsync(jsonPath);

      var permissions = JsonSerializer.Deserialize<List<Permission>>(jsonString, JsonSerializeSeeder.options);
      var newPermissions = new List<Permission>();

      try
      {
        await dbContext.Database.BeginTransactionAsync();

        foreach (var permission in permissions)
        {
          newPermissions.Add(permission);
        }
        await dbContext.Permissions.AddRangeAsync(newPermissions);
        await dbContext.SaveChangesAsync();
        await dbContext.Database.CommitTransactionAsync();
      }
      catch (Exception e)
      {
        logger.LogError(e, "Error while seeding Permissions");
        await dbContext.Database.RollbackTransactionAsync();
      }

      logger.LogInformation("Seeding Permissions complete");
    }
  }
}