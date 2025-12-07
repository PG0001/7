using EventHubLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories.Interfaces
{
    public interface IScheduledJobRepository
    {
     Task AddScheduledJobAsync(ScheduledJob job);
        Task<IEnumerable<ScheduledJob>> GetPendingJobsAsync();
     Task UpdateAsync(ScheduledJob job);
        Task<IEnumerable<ScheduledJob>> GetAllAsync();

    }
}
