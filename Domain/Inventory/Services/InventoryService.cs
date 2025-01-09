using InventoryService.Infrastructure.Exceptions;
using InventoryService.Infrastructure.Dtos;
using InventoryService.Domain.Inventory.Repositories;
using InventoryService.Domain.Inventory.Dtos;
using InventoryService.Domain.Inventory.Messages;
using InventoryService.Infrastructure.Shareds;

namespace InventoryService.Domain.Inventory.Services
{
    public class InventoryService(
        InventoryQueryRepository InventoryQueryRepository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        private readonly InventoryQueryRepository _inventoryQueryRepository = InventoryQueryRepository;

        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<PaginationModel<InventoryResultDto>> Index(InventoryQueryDto query)
        {
            var userId = Utils.GetUserLoggedId(_httpContextAccessor);
            var result = await _inventoryQueryRepository.Pagination(query);
            var formattedResult = InventoryResultDto.MapRepo(result.Data);
            var paginate = PaginationModel<InventoryResultDto>.Parse(formattedResult, result.Count, query);
            return paginate;
        }

        public async Task<InventoryResultDto> DetailById(Guid id, Guid userId)
        {
            var inventory = await _inventoryQueryRepository.FindOneById(id);

            if (inventory == null)
            {
                throw new DataNotFoundException(InventoryErrorMessage.ErrInventoryNotFound);
            }

            return new InventoryResultDto(inventory);
        }

        public async Task<bool> IsQuantityAvailable(InventoryNatsDto inventoryNatsDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inventoryNatsDto.ProductName))
                    throw new BadHttpRequestException("Product name cannot be null or empty.");

                if (inventoryNatsDto.Quantity <= 0)
                    throw new BadHttpRequestException("Required quantity must be greater than zero.");

                return await _inventoryQueryRepository.IsQuantityAvailable(inventoryNatsDto.ProductName, inventoryNatsDto.Quantity);
            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException($"Inventory check failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}