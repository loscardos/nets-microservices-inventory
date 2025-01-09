using document_generator.Infrastructure.Helpers;
using InventoryService.Domain.Inventory.Dtos;
using InventoryService.Domain.Inventory.Messages;
using InventoryService.Infrastructure.Helpers;
using InventoryService.Infrastructure.Subscriptions;
using Microsoft.VisualBasic.CompilerServices;
using Utils = InventoryService.Infrastructure.Shareds.Utils;

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
                InventoryNatsDto inventoryNatsDto = Utils.JsonDeserialize<InventoryNatsDto>(data["data"].ToString());

                if (string.IsNullOrWhiteSpace(inventoryNatsDto.ProductName))
                    throw new ArgumentException(InventoryErrorMessage.ErrProductNameRequired);

                if (inventoryNatsDto.Quantity <= 0)
                    throw new ArgumentException(InventoryErrorMessage.ErrGreaterThenZero);

                bool isAvailable = await _inventoryService.IsQuantityAvailable(inventoryNatsDto);

                return isAvailable
                    ? Utils.SuccessResponseFormat(string.Format(InventorySuccessMessage.SuccessInventoryReady,
                        inventoryNatsDto.ProductName,
                        inventoryNatsDto.Quantity))
                    : Utils.ErrorResponseFormat(string.Format(InventoryErrorMessage.ErrInsufficientInventory,
                        inventoryNatsDto.ProductName,
                        inventoryNatsDto.Quantity));
            }
            catch (ArgumentException ex)
            {
                return Utils.ErrorResponseFormat(ex.Message);
            }
            catch (Exception ex)
            {
                return Utils.ErrorResponseFormat(ex.Message);
            }
        }
    }
}