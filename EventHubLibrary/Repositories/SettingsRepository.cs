using EventHubLibrary.Models;
using Microsoft.EntityFrameworkCore;
using EventHubLibrary.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DBContext _context;

        public SettingsRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<NotificationSetting?> GetAsync()
        {
            return await _context.NotificationSettings.FirstOrDefaultAsync();
        }
        public async Task AddAsync(NotificationSetting setting)
        {
            await _context.NotificationSettings.AddAsync(setting);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(NotificationSetting setting)
        {
            _context.NotificationSettings.Update(setting);
            await _context.SaveChangesAsync();
        }
    }
}
