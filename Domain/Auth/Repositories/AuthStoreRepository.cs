using InventoryService.Infrastructure.Databases;

namespace InventoryService.Domain.Auth.Repositories
{
    public class AuthStoreRepository(
        IamDBContext context
        )
    {
        private readonly IamDBContext _context = context;

        public async Task Create(Models.User data)
        {
            _context.Users.Add(data);
            await _context.SaveChangesAsync();
        }
    }
}