using Microsoft.AspNetCore.Mvc;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Controllers;

public class HomeController : Controller
{
    private readonly IInstanceRepository _repo;

    public HomeController(IInstanceRepository repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        var instances = _repo.GetAll();
        return View(instances);
    }

    public IActionResult About() => View();
}
