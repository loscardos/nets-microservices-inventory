using document_generator.Infrastructure.Helpers;
using InventoryService.Domain.Inventory.Dtos;
using InventoryService.Domain.Inventory.Messages;
using InventoryService.Infrastructure.Helpers;
using InventoryService.Infrastructure.Subscriptions;

namespace InventoryService.Domain.Inventory.Listeners
{
    public class InventoryWithReplyListener(Services.InventoryService inventoryService)
        : IReplyAsyncAction<IDictionary<string, object>, IDictionary<string, object>>
    {
        private readonly Services.InventoryService _inventoryService = inventoryService;

        public async Task<IDictionary<string, object>> ReplyAsync(IDictionary<string, object> data)
        {
            try
            {
                InventoryNatsDto inventoryNatsDto = data.ToObject<InventoryNatsDto>();

                if (string.IsNullOrWhiteSpace(inventoryNatsDto.ProductName))
                    throw new ArgumentException(InventoryErrorMessage.ErrProductNameRequired);

                if (inventoryNatsDto.Quantity <= 0)
                    throw new ArgumentException(InventoryErrorMessage.ErrGreaterThenZero);

                bool isAvailable = await _inventoryService.IsQuantityAvailable(inventoryNatsDto);

                return isAvailable
                    ? ResponseBuilder.SuccessResponse(string.Format(InventorySuccessMessage.SuccessInventoryReady,
                            inventoryNatsDto.ProductName,
                            inventoryNatsDto.Quantity)
                        , null).ToDictionary()
                    : ResponseBuilder.ErrorResponse(400, string.Format(InventoryErrorMessage.ErrInsufficientInventory,
                            inventoryNatsDto.ProductName,
                            inventoryNatsDto.Quantity),
                        null).ToDictionary();
            }
            catch (ArgumentException ex)
            {
                return ResponseBuilder.ErrorResponse(400, "Invalid input", ex.Message)
                    .ToDictionary();
            }
            catch (Exception ex)
            {
                return ResponseBuilder.ErrorResponse(500, "An unexpected error occurred", ex.Message)
                    .ToDictionary();
            }
        }
    }
}