using Microsoft.AspNetCore.Mvc;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Controllers;

public class CalendarController : Controller
{
    private readonly IInstanceRepository _instanceRepo;

    public CalendarController(IInstanceRepository instanceRepo)
    {
        _instanceRepo = instanceRepo;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Details(string instanceName, string itemName)
    {
        var instance = _instanceRepo.GetInstance(instanceName);
        if (instance == null) return View("NotFound");

        var calRepo = new CalendarRepository(instance);
        var cal = await calRepo.GetCalendar(itemName);
        ViewData["calendarName"] = itemName;
        return cal == null ? View("NotFound") : View(cal);
    }
}
