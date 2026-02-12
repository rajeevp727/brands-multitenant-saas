using System.Threading.Tasks;
using GreenPantry.Domain.Entities;
using GreenPantry.Application.Interfaces;
using GreenPantry.Infrastructure.Data;

namespace GreenPantry.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            await _context.Set<Notification>().AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }
    }
}
