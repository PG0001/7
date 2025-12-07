using EventHubLibrary.Models;
using EventHubLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly DBContext _context;

        public NotificationTemplateRepository(DBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NotificationTemplate template)
        {
            await _context.NotificationTemplates.AddAsync(template);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var t = await _context.NotificationTemplates.FindAsync(id);
            if (t != null)
            {
                _context.NotificationTemplates.Remove(t);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<NotificationTemplate>> GetAllAsync()
        {
            return await _context.NotificationTemplates
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<NotificationTemplate?> GetByIdAsync(int id)
        {
            return await _context.NotificationTemplates.FindAsync(id);
        }

        public async Task<NotificationTemplate?> GetByNameAsync(string templateName)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.TemplateName.ToLower() == templateName.ToLower());
        }

        public async Task<bool> ExistsAsync(string templateName)
        {
            return await _context.NotificationTemplates
                .AnyAsync(t => t.TemplateName.ToLower() == templateName.ToLower());
        }

        public async Task UpdateAsync(NotificationTemplate template)
        {
            _context.NotificationTemplates.Update(template);
            await _context.SaveChangesAsync();
        }
    }
}
