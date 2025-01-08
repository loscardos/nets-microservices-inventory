using InventoryService.Infrastructure.Databases;

namespace InventoryService.Infrastructure.Seeders
{
  public interface ISeeder
  {
    Task Seed(IamDBContext dbContext, ILogger logger);
  }
}
