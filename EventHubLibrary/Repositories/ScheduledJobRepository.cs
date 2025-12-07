using EventHubLibrary.Models;
using EventHubLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories
{
    public class ScheduledJobRepository : IScheduledJobRepository
    {
        private readonly DBContext _context;

        public ScheduledJobRepository(DBContext context)
        {
            _context = context;
        }

        public async Task AddScheduledJobAsync(ScheduledJob job)
        {
            await _context.ScheduledJobs.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ScheduledJob>> GetPendingJobsAsync()
        {
            var pendingJobs = await _context.ScheduledJobs
     .Where(s => (s.IsTriggered == null || s.IsTriggered == false) && s.ScheduledTime <= DateTime.UtcNow)
     .ToListAsync();
            return pendingJobs;

        }

        // ✅ New method to fetch all jobs
        public async Task<IEnumerable<ScheduledJob>> GetAllAsync()
        {
            return await _context.ScheduledJobs
                .OrderByDescending(j => j.ScheduledTime)
                .ToListAsync();
        }

        public async Task UpdateAsync(ScheduledJob job)
        {
            _context.ScheduledJobs.Update(job);
            await _context.SaveChangesAsync();
        }
    }
}
