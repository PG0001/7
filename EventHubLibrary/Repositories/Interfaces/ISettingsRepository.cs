using EventHubLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        Task<NotificationSetting?> GetAsync(); // get the single settings row
        Task UpdateAsync(NotificationSetting setting);
    }
}
