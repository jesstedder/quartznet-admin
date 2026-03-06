using Quartz;

namespace QuartzAdmin.web.Models;

public class TriggerFireTimesModel
{
    public ICalendar? Calendar { get; set; }
    public ITrigger? Trigger { get; set; }
    public InstanceModel? Instance { get; set; }
}
