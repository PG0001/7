using EventHubLibrary.Models;
using EventHubLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DBContext _context;

        public NotificationRepository(DBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var n = await _context.Notifications.FindAsync(id);
            if (n != null)
            {
                _context.Notifications.Remove(n);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
        }

        public async Task<IEnumerable<Notification>> GetByUserAsync(int userId, bool includeRead = false)
        {
            var query = _context.Notifications.Where(n => n.UserId == userId);

            if (!includeRead)
                query = query.Where(n => n.IsRead == false || n.IsRead == null);

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetLatestByUserAsync(int userId, int limit = 5)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task UpdateAsync(Notification notif)
        {
            _context.Notifications.Update(notif);
            await _context.SaveChangesAsync();
        }
    }
}
