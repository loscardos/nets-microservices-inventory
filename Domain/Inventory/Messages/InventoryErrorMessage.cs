namespace InventoryService.Domain.Inventory.Messages
{
    public static class InventoryErrorMessage
    {
        public const string ErrInventoryNotFound = "Inventory not found";
        public const string ErrProductNameRequired = "Product name is required.";
        public const string ErrGreaterThenZero = "Quantity must be greater than zero.";
        public const string ErrInsufficientInventory = "Insufficient inventory for {0}. Required: {1}.";
        
    }

}
