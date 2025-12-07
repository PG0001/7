using EventHubLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories.Interfaces
{
    public interface INotificationTemplateRepository
    {
        Task<NotificationTemplate?> GetByIdAsync(int id);
        Task<NotificationTemplate?> GetByNameAsync(string templateName);
        Task<IEnumerable<NotificationTemplate>> GetAllAsync();
        Task AddAsync(NotificationTemplate template);
        Task UpdateAsync(NotificationTemplate template);
        Task DeleteAsync(int id);
    }
}
