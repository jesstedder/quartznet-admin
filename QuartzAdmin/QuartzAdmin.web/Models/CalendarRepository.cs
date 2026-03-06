using Quartz;

namespace QuartzAdmin.web.Models;

public class CalendarRepository
{
    private readonly InstanceModel _quartzInstance;

    public CalendarRepository(InstanceModel instance)
    {
        _quartzInstance = instance;
    }

    public async Task<ICalendar?> GetCalendar(string calendarName)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        return await sched.GetCalendar(calendarName);
    }
}
