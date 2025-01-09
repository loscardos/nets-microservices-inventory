namespace InventoryService.Domain.Inventory.Dtos
{
    public class InventoryNatsDto
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public Guid UserId { get; set; }
    }
}