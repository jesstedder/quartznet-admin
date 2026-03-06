using Quartz;

namespace QuartzAdmin.web.Models;

public class JobRepository
{
    private readonly InstanceModel _quartzInstance;

    public JobRepository(InstanceModel instance)
    {
        _quartzInstance = instance;
    }

    public async Task<IJobDetail?> GetJob(string jobName, string groupName)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        return await sched.GetJobDetail(new JobKey(jobName, groupName));
    }

    public async Task RunJobNow(string jobName, string groupName)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        await sched.TriggerJob(new JobKey(jobName, groupName));
    }

    public async Task RunJobNow(string jobName, string groupName, JobDataMap jdm)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        await sched.TriggerJob(new JobKey(jobName, groupName), jdm);
    }

    public async Task DeleteJob(string jobName, string groupName)
    {
        var sched = await _quartzInstance.GetQuartzScheduler();
        await sched.DeleteJob(new JobKey(jobName, groupName));
    }
}
