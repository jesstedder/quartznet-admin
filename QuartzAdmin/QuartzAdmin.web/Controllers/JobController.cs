using Microsoft.AspNetCore.Mvc;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Controllers;

public class JobController : Controller
{
    private readonly IInstanceRepository _instanceRepo;

    public JobController(IInstanceRepository instanceRepo)
    {
        _instanceRepo = instanceRepo;
    }

    public async Task<IActionResult> Index(string id)
    {
        var instance = _instanceRepo.GetInstance(id);
        if (instance == null) return View("NotFound");

        ViewData["instanceName"] = instance.InstanceName;
        var jobs = await instance.GetAllJobs();
        if (jobs == null || jobs.Count == 0)
            return View("NotFound");
        return View(jobs);
    }

    public async Task<IActionResult> Details(string instanceName, string groupName, string itemName)
    {
        var instance = _instanceRepo.GetInstance(instanceName);
        if (instance == null) return View("NotFound");

        var jobRepo = new JobRepository(instance);
        var triggerRepo = new TriggerRepository(instance);

        var job = await jobRepo.GetJob(itemName, groupName);
        var jvm = new JobViewModel { JobDetail = job };
        if (job != null)
            jvm.Triggers = await triggerRepo.GetTriggersForJob(itemName, groupName);

        ViewData["instanceName"] = instanceName;
        return job == null ? View("NotFound") : View(jvm);
    }
}
