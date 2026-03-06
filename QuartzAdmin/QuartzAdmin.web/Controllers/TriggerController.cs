using Microsoft.AspNetCore.Mvc;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Controllers;

public class TriggerController : Controller
{
    private readonly IInstanceRepository _instanceRepo;

    public TriggerController(IInstanceRepository instanceRepo)
    {
        _instanceRepo = instanceRepo;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Details(string instanceName, string groupName, string itemName)
    {
        var instance = _instanceRepo.GetInstance(instanceName);
        if (instance == null) return View("NotFound");

        var trigRepo = new TriggerRepository(instance);
        var m = new TriggerFireTimesModel();
        m.Trigger = await trigRepo.GetTrigger(itemName, groupName);
        if (m.Trigger == null)
        {
            ViewData["triggerName"] = itemName;
            return View("NotFound");
        }

        var calRepo = new CalendarRepository(instance);
        m.Calendar = await calRepo.GetCalendar(m.Trigger.CalendarName);
        m.Instance = instance;
        ViewData["groupName"] = groupName;
        return View(m);
    }

    public async Task<IActionResult> FireTimes(string instanceName, string groupName, string itemName)
    {
        var instance = _instanceRepo.GetInstance(instanceName);
        if (instance == null) return View("NotFound");

        var trigRepo = new TriggerRepository(instance);
        var m = new TriggerFireTimesModel();
        m.Trigger = await trigRepo.GetTrigger(itemName, groupName);
        if (m.Trigger == null)
        {
            ViewData["triggerName"] = itemName;
            return View("NotFound");
        }

        var calRepo = new CalendarRepository(instance);
        m.Calendar = await calRepo.GetCalendar(m.Trigger.CalendarName);
        ViewData["groupName"] = groupName;
        return View(m);
    }
}
