using System;
using System.Collections.Generic;

namespace EventHubLibrary.Models;

public partial class NotificationTemplate
{
    public int TemplateId { get; set; }

    public string TemplateName { get; set; } = null!;

    public string? Subject { get; set; }

    public string Body { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
