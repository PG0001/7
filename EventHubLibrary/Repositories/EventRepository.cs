using EventHubLibrary.Models;
using EventHubLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly DBContext _context;

        public EventRepository(DBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Event evt)
        {
            await _context.Events.AddAsync(evt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt != null)
            {
                _context.Events.Remove(evt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                    .Include(e => e.Notifications)
                    .Include(e => e.ScheduledJobs)
                    .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task<IEnumerable<Event>> GetByUserAsync(int userId)
        {
            return await _context.Events.Where(e => e.UserId == userId)
                .OrderByDescending(e => e.StartTime)
                .ToListAsync();
        }

        public async Task UpdateAsync(Event evt)
        {
            _context.Events.Update(evt);
            await _context.SaveChangesAsync();
        }
    }
}
