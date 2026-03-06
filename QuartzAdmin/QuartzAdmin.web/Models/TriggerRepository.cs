using Quartz;
using Quartz.Impl.Matchers;

namespace QuartzAdmin.web.Models;

public class TriggerRepository
{
    private readonly InstanceModel _quartzInstance;

    public TriggerRepository(InstanceModel instance)
    {
        _quartzInstance = instance;
    }

    public async Task<ITrigger?> GetTrigger(string triggerName, string groupName)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        return await sched.GetTrigger(new TriggerKey(triggerName, groupName));
    }

    public async Task<IList<TriggerStatusModel>> GetAllTriggerStatus(string groupName)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        var keys = await sched.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(groupName));
        var triggerStatuses = new List<TriggerStatusModel>();

        foreach (var key in keys)
        {
            var trig = await sched.GetTrigger(key);
            if (trig == null) continue;
            var st = await sched.GetTriggerState(key);
            var nextFireTime = trig.GetNextFireTimeUtc();
            var lastFireTime = trig.GetPreviousFireTimeUtc();

            triggerStatuses.Add(new TriggerStatusModel
            {
                TriggerName = key.Name,
                GroupName = groupName,
                State = st,
                NextFireTime = nextFireTime.HasValue ? nextFireTime.Value.ToLocalTime().ToString() : "",
                LastFireTime = lastFireTime.HasValue ? lastFireTime.Value.ToLocalTime().ToString() : "",
                JobName = trig.JobKey.Name
            });
        }
        return triggerStatuses;
    }

    public async Task<IList<TriggerStatusModel>> GetAllTriggerStatus()
    {
        var groups = await _quartzInstance.FindAllGroups();
        var triggerStatuses = new List<TriggerStatusModel>();
        foreach (var group in groups)
        {
            triggerStatuses.AddRange(await GetAllTriggerStatus(group));
        }
        return triggerStatuses;
    }

    public async Task<IList<ITrigger>> GetTriggersForJob(string jobName, string groupName)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        var triggers = await sched.GetTriggersOfJob(new JobKey(jobName, groupName));
        return triggers.ToList();
    }
}
