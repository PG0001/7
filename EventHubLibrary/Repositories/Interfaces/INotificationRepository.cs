using EventHubLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId);

        Task<Notification?> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetByUserAsync(int userId, bool includeRead = false);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(int id);
    }
}
