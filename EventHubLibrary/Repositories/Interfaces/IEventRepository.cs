using EventHubLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubLibrary.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(int id);
        Task<IEnumerable<Event>> GetAllAsync();
        Task<IEnumerable<Event>> GetByUserAsync(int userId);
        Task AddAsync(Event evt);
        Task UpdateAsync(Event evt);
        Task DeleteAsync(int id);
    }
}
