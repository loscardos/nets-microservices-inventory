using InventoryService.Infrastructure.Dtos;

namespace InventoryService.Domain.Inventory.Dtos
{
    public class InventoryQueryDto : QueryDto
    {
        public string ProductName { get; set; }

    }
}
