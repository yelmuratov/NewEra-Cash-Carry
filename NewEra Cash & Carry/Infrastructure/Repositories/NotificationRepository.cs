using NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces;
using NewEra_Cash___Carry.Infrastructure.Data;
using NewEra_Cash___Carry.Core.Entities;

namespace NewEra_Cash___Carry.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly RetailOrderingSystemDbContext _context;

        public NotificationRepository(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

        public async Task SaveNotificationAsync(string message, string type)
        {
            var notification = new Notification
            {
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
    }
}
