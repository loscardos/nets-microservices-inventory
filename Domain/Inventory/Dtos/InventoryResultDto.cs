namespace InventoryService.Domain.Inventory.Dtos
{
    public class InventoryResultDto : Models.Inventory
    {
        public InventoryResultDto(Models.Inventory inventory)
        {
            Id = inventory.Id;
            ProductName = inventory.ProductName;
            Quantity = inventory.Quantity;
            PricePerUnit = inventory.PricePerUnit;
            CreatedAt = inventory.CreatedAt;
            UpdatedAt = inventory.UpdatedAt;
        }

        public static List<InventoryResultDto> MapRepo(List<Models.Inventory> data)
        {
            return data?.Select(inventory => new InventoryResultDto(inventory)).ToList();
        }
    }
}