using InventoryService.Infrastructure.Databases;
using InventoryService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Domain.Notification.Repositories
{
    public class NotificationStoreRepository(
        IamDBContext context
        )
    {
        private readonly IamDBContext _context = context;

        public async Task ReadNotificationById(Guid id, Guid userId)
        {
            try
            {
                await _context.Notifications.Where(n => n.Id == id && n.UserId == userId).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No notification was read.");
            }
        }

        public async Task ReadAllNotificationByUserId(Guid userId)
        {
            try
            {
                await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No notification was read.");
            }
        }
    }
}