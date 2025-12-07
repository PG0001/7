using EventHubLibrary.Models;
using EventHubLibrary.Repositories;
using EventHubLibrary.Repositories.Interfaces;
using EventHubWebApi.Dtos;

using EventHubWebApi.Services;
using EventHubWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventHubWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepo;
        private readonly INotificationService _notifService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventRepository eventRepo, INotificationService notifService, ILogger<EventsController> logger)
        {
            _eventRepo = eventRepo;
            _notifService = notifService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
        {
            try
            {
                var evt = new Event
                {
                    UserId = dto.UserId,
                    EventName = dto.EventName,
                    Description = dto.Description,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime
                };

                await _eventRepo.AddAsync(evt);
                _logger.LogInformation("Event created: Id={EventId}, User={UserId}", evt.EventId, dto.UserId);

                await _notifService.ScheduleEventRemindersAsync(evt);
                _logger.LogInformation("Event reminders scheduled for EventId={EventId}", evt.EventId);

                // Return DTO instead of EF entity
                var response = new EventResponseDto
                {
                    EventId = evt.EventId,
                    UserId = evt.UserId,
                    EventName = evt.EventName,
                    Description = evt.Description,
                    StartTime = evt.StartTime,
                    EndTime = evt.EndTime,
                    CreatedAt = evt.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event for User={UserId}", dto.UserId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetEventsByUser(int userId)
        {
            try
            {
                var events = await _eventRepo.GetByUserAsync(userId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events for User={UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
