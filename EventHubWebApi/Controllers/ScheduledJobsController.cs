using EventHubLibrary.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventHubWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduledJobsController : ControllerBase
    {
        private readonly ScheduledJobRepository _scheduledJobRepo;
        private readonly ILogger<ScheduledJobsController> _logger;

        public ScheduledJobsController(ScheduledJobRepository scheduledJobRepo, ILogger<ScheduledJobsController> logger)
        {
            _scheduledJobRepo = scheduledJobRepo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var jobs = await _scheduledJobRepo.GetAllAsync();
                _logger.LogInformation("Fetched all scheduled jobs, Count={Count}", jobs.Count());
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching scheduled jobs");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
