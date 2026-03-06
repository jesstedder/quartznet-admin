using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace QuartzAdmin.web.Models;

public class InstanceModel
{
    public InstanceModel()
    {
        InstanceProperties = new List<InstancePropertyModel>();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InstanceID { get; set; }

    [Required]
    public string InstanceName { get; set; } = string.Empty;

    public ICollection<InstancePropertyModel> InstanceProperties { get; set; }

    private IScheduler? _currentScheduler;

    public async Task<IScheduler> GetQuartzScheduler()
    {
        if (_currentScheduler == null)
        {
            var props = new System.Collections.Specialized.NameValueCollection();
            foreach (var prop in InstanceProperties)
            {
                props.Add(prop.PropertyName, prop.PropertyValue);
            }
            ISchedulerFactory sf = new StdSchedulerFactory(props);
            _currentScheduler = await sf.GetScheduler();
        }
        return _currentScheduler;
    }

    public async Task<IEnumerable<string>> FindAllGroups()
    {
        var scheduler = await GetQuartzScheduler();
        var jobGroups = await scheduler.GetJobGroupNames();
        var triggerGroups = await scheduler.GetTriggerGroupNames();
        return jobGroups.Union(triggerGroups).Distinct();
    }

    public async Task<List<IJobDetail>> GetAllJobs(string groupName)
    {
        var jobs = new List<IJobDetail>();
        var scheduler = await GetQuartzScheduler();
        var keys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName));
        foreach (var key in keys)
        {
            var job = await scheduler.GetJobDetail(key);
            if (job != null) jobs.Add(job);
        }
        return jobs;
    }

    public async Task<List<IJobDetail>> GetAllJobs()
    {
        var jobs = new List<IJobDetail>();
        var groups = await FindAllGroups();
        foreach (var group in groups)
        {
            jobs.AddRange(await GetAllJobs(group));
        }
        return jobs;
    }

    public async Task<List<ITrigger>> GetAllTriggers(string groupName)
    {
        var triggers = new List<ITrigger>();
        var scheduler = await GetQuartzScheduler();
        var keys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(groupName));
        foreach (var key in keys)
        {
            var trigger = await scheduler.GetTrigger(key);
            if (trigger != null) triggers.Add(trigger);
        }
        return triggers;
    }

    public async Task<List<ITrigger>> GetAllTriggers()
    {
        var triggers = new List<ITrigger>();
        var groups = await FindAllGroups();
        foreach (var group in groups)
        {
            triggers.AddRange(await GetAllTriggers(group));
        }
        return triggers;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(InstanceName) && InstanceProperties.Count > 0;
    }
}
