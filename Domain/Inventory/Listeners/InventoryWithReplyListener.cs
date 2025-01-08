using InventoryService.Infrastructure.Subscriptions;

namespace InventoryService.Domain.Inventory.Listeners
{
    public class InventoryWithReplyListener(Services.InventoryService inventoryService)
        : IReplyAsyncAction<IDictionary<string, object>, IDictionary<string, object>>
    {
        private readonly Services.InventoryService _inventoryService = inventoryService;

        public async Task<IDictionary<string, object>> ReplyAsync(IDictionary<string, object> data)
        {
            Dictionary<string, object> reply;

            try
            {
                string productName = data.ContainsKey("productName") ? data["productName"]?.ToString() : null;
                int requiredQuantity =
                    data.ContainsKey("quantity") && int.TryParse(data["quantity"]?.ToString(), out var qty) ? qty : 0;

                if (string.IsNullOrWhiteSpace(productName))
                    throw new ArgumentException("Product name is required.");

                if (requiredQuantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero.");

                bool isAvailable = await _inventoryService.IsQuantityAvailable(productName, requiredQuantity);

                if (isAvailable)
                {
                    reply = new()
                    {
                        { "status", "OK" },
                        { "message", $"Inventory is ready for {productName} with quantity {requiredQuantity}." }
                    };
                }
                else
                {
                    reply = new()
                    {
                        { "status", "ERROR" },
                        { "message", $"Insufficient inventory for {productName}. Required: {requiredQuantity}." }
                    };
                }
            }
            catch (ArgumentException ex)
            {
                reply = new() { { "status", "ERROR" }, { "message", $"Validation error: {ex.Message}" } };
            }
            catch (Exception ex)
            {
                reply = new() { { "status", "ERROR" }, { "message", $"Unexpected error: {ex.Message}" } };
            }

            return reply;
        }
    }
}