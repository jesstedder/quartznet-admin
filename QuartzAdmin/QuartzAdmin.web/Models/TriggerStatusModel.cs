using Quartz;

namespace QuartzAdmin.web.Models;

public class TriggerStatusModel
{
    public string NextFireTime { get; set; } = string.Empty;
    public string LastFireTime { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string TriggerName { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public TriggerState State { get; set; }
    public string StateDesc => State.ToString();
}
