using Microsoft.AspNetCore.Mvc;
using QuartzAdmin.web.Models;
using QuartzAdmin.web.Helpers;

namespace QuartzAdmin.web.Controllers;

public class ConnectionController : Controller
{
    private readonly IConnectionRepository _connectionRepository;

    public ConnectionController(IConnectionRepository repository)
    {
        _connectionRepository = repository;
    }

    public IActionResult Index() => View(_connectionRepository.GetConnections());

    public IActionResult Details(int id) => View();

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(IFormCollection collection)
    {
        ConnectionModel? connection = null;
        try
        {
            connection = new ConnectionModel();
            connection.Name = collection["Name"];
            connection.ConnectionParameters.Clear();
            connection.ConnectionParameters.AddRange(ConnectionParameterModel.FromFormCollection(collection));

            if (connection.IsValid)
            {
                if (!_connectionRepository.IsValid(connection, out var ruleViolations))
                {
                    ModelState.AddRuleViolations(ruleViolations);
                    return View(connection);
                }
                _connectionRepository.AddConnection(connection);
                _connectionRepository.Save();
            }
            else
            {
                ModelState.AddRuleViolations(connection.GetRuleViolations());
                return View(connection);
            }
            return RedirectToAction("Index");
        }
        catch
        {
            return View();
        }
    }

    public IActionResult Edit(int id)
    {
        var connection = _connectionRepository.GetConnection(id);
        if (connection == null)
            return View("NotFound", "The specified connection was not found");
        return View(connection);
    }

    [HttpPost]
    public IActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            var connection = _connectionRepository.GetConnection(id);
            if (connection == null)
                return View("NotFound", "The specified connection was not found");

            connection.Name = collection["Name"];
            connection.ConnectionParameters.Clear();
            connection.ConnectionParameters.AddRange(ConnectionParameterModel.FromFormCollection(collection));

            if (connection.IsValid)
            {
                if (!_connectionRepository.IsValid(connection, out var ruleViolations))
                {
                    ModelState.AddRuleViolations(ruleViolations);
                    return View(connection);
                }
                _connectionRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddRuleViolations(connection.GetRuleViolations());
                return View(connection);
            }
        }
        catch
        {
            return View();
        }
    }
}
