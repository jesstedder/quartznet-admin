using Microsoft.AspNetCore.Mvc;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Controllers;

public class JobExecutionController : Controller
{
    public IInstanceRepository Repository { get; set; }

    public JobExecutionController(IInstanceRepository repository)
    {
        Repository = repository;
    }

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> RunNow(string instanceName, string groupName, string itemName)
    {
        var instance = Repository.GetByName(instanceName);
        if (instance == null) return Content("Instance not found");

        var jobRepo = new JobRepository(instance);
        var job = await jobRepo.GetJob(itemName, groupName);
        if (job == null) return Content("Job not found");

        var jdm = job.JobDataMap;
        foreach (string jdmKey in Request.Form.Keys)
        {
            if (jdmKey.StartsWith("jdm_"))
            {
                var dataKey = jdmKey[4..];
                if (jdm.ContainsKey(dataKey))
                {
                    jdm[dataKey] = Convert.ChangeType(Request.Form[jdmKey].ToString(), jdm[dataKey]!.GetType());
                }
            }
        }
        await jobRepo.RunJobNow(itemName, groupName, jdm);
        return Content("Job execution started");
    }

    public IActionResult CurrentStatus(string id)
    {
        ViewData["groupName"] = id;
        return View();
    }

    public async Task<JsonResult> GetCurrentTriggerStatusList(string id)
    {
        var instance = Repository.GetByName(id);
        if (instance == null) return Json(new List<TriggerStatusModel>());
        var trigRepo = new TriggerRepository(instance);
        var triggerStatuses = await trigRepo.GetAllTriggerStatus();
        return Json(triggerStatuses);
    }
}
