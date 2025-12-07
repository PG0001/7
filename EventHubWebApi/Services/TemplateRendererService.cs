using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace EventHubWebApi.Services
{
    public class TemplateRendererService
    {
        private readonly ILogger<TemplateRendererService> _logger;

        public TemplateRendererService(ILogger<TemplateRendererService> logger)
        {
            _logger = logger;
        }

        public string Render(string template, Dictionary<string, string>? placeholders)
        {
            if (string.IsNullOrEmpty(template) || placeholders == null || placeholders.Count == 0)
                return template ?? "";

            var result = template;
            foreach (var kv in placeholders)
            {
                try
                {
                    result = result.Replace("{{" + kv.Key + "}}", kv.Value ?? "", StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to replace placeholder {Key} in template", kv.Key);
                }
            }
            _logger.LogInformation(result + "Data");
            _logger.LogInformation("Template rendered successfully.");
            return result;
        }
    }
}
