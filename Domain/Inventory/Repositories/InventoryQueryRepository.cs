using System.Linq.Expressions;
using InventoryService.Domain.Inventory.Dtos;
using InventoryService.Infrastructure.Databases;
using InventoryService.Infrastructure.Dtos;
using InventoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Domain.Inventory.Repositories
{
    public class InventoryQueryRepository(
        IamDBContext context
    )
    {
        private readonly IamDBContext _context = context;

        public async Task<PaginationResult<Models.Inventory>> Pagination(InventoryQueryDto queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _context.Inventories
                .AsQueryable()
                .AsNoTracking();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = await query.Skip(skip).Take(queryParams.PerPage).ToListAsync();
            var count = await Count(query);

            return new PaginationResult<Models.Inventory> { Data = data, Count = count, };
        }

        private static IQueryable<Models.Inventory> QuerySearch(IQueryable<Models.Inventory> query,
            InventoryQueryDto queryParams)
        {
            if (queryParams.Search != null)
            {
                query = query.Where(data =>
                    data.ProductName.Contains(queryParams.Search));
            }

            return query;
        }

        private static IQueryable<Models.Inventory> QueryFilter(IQueryable<Models.Inventory> query,
            InventoryQueryDto queryParams)
        {
            if (queryParams.ProductName != null)
            {
                query = query.Where(data => data.ProductName.Equals(queryParams.ProductName));
            }

            return query;
        }

        private static IQueryable<Models.Inventory> QuerySort(IQueryable<Models.Inventory> query,
            InventoryQueryDto queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Inventory, object>>> sortFunctions = new()
            {
                { "name", data => data.ProductName },
                { "updated_at", data => data.UpdatedAt },
                { "created_at", data => data.CreatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Inventory, object>> value))
            {
                throw new BadHttpRequestException(
                    $"Invalid sort column: {queryParams.SortBy}, available sort columns: " +
                    string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrder.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> Count(IQueryable<Models.Inventory> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }

        public async Task<Models.Inventory> FindOneById(Guid id = default)
        {
            return await _context.Inventories
                .Where(data => data.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<List<Models.Inventory>> Get(string search, int page, int perPage)
        {
            int skip = (1 - page) * perPage;
            List<Models.Inventory> permissions;
            IQueryable<Models.Inventory> permissionQuery = _context.Inventories;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.ProductName.Contains(search));
            }

            permissions = await permissionQuery.Skip(skip).Take(perPage).ToListAsync();

            return permissions;
        }

        public async Task<int> CountAll(string search)
        {
            IQueryable<Models.Inventory> permissionQuery = _context.Inventories;
            if (search != null)
            {
                permissionQuery = permissionQuery.Where(permission => permission.ProductName.Contains(search));
            }

            return await permissionQuery.CountAsync();
        }
        
        public async Task<bool> IsQuantityAvailable(string productName, int requiredQuantity)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(p => p.ProductName == productName);

            if (inventory == null)
                throw new BadHttpRequestException($"inventory with name '{productName}' does not exist.");

            return inventory.Quantity >= requiredQuantity;
        }
    }
}