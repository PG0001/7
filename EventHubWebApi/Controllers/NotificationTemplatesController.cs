using EventHubLibrary.Repositories;
using EventHubLibrary.Repositories.Interfaces;
using EventHubWebApi.Dtos;
using EventHubLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventHubWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationTemplatesController : ControllerBase
    {
        private readonly INotificationTemplateRepository _templateRepo;
        private readonly ILogger<NotificationTemplatesController> _logger;

        public NotificationTemplatesController(INotificationTemplateRepository templateRepo, ILogger<NotificationTemplatesController> logger)
        {
            _templateRepo = templateRepo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var templates = await _templateRepo.GetAllAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notification templates");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationTemplateDto dto)
        {
            try
            {
                var template = new NotificationTemplate
                {
                    TemplateName = dto.TemplateName,
                    Subject = dto.Subject,
                    Body = dto.Body
                };

                await _templateRepo.AddAsync(template);
                _logger.LogInformation("Notification template created: Id={TemplateId}", template.TemplateId);

                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification template");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NotificationTemplateDto dto)
        {
            try
            {
                var template = await _templateRepo.GetByIdAsync(id);
                if (template == null) return NotFound();

                template.TemplateName = dto.TemplateName;
                template.Subject = dto.Subject;
                template.Body = dto.Body;

                await _templateRepo.UpdateAsync(template);
                _logger.LogInformation("Notification template updated: Id={TemplateId}", id);

                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification template: Id={TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _templateRepo.DeleteAsync(id);
                _logger.LogInformation("Notification template deleted: Id={TemplateId}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification template: Id={TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
