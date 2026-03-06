using Microsoft.AspNetCore.Mvc;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Controllers;

public class InstanceController : Controller
{
    public IInstanceRepository Repository { get; set; }

    public InstanceController(IInstanceRepository repository)
    {
        Repository = repository;
    }

    public IActionResult Index() => View(Repository.GetAll());

    public IActionResult Details(string id)
    {
        var instance = Repository.GetByName(id);
        return instance == null ? View("NotFound") : View(instance);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(IFormCollection collection)
    {
        var instance = new InstanceModel();
        instance.InstanceName = collection["InstanceName"].ToString() ?? string.Empty;

        foreach (string key in collection.Keys)
        {
            if (key.Contains("InstancePropertyKey") && collection[key].ToString().Length > 0)
            {
                var propIdx = key.Replace("InstancePropertyKey-", "");
                instance.InstanceProperties.Add(new InstancePropertyModel
                {
                    ParentInstance = instance,
                    PropertyName = collection[key].ToString(),
                    PropertyValue = collection["InstancePropertyValue-" + propIdx].ToString()
                });
            }
        }
        Repository.Save(instance);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(string id)
    {
        var instance = Repository.GetByName(id);
        return View(instance);
    }

    [HttpPost]
    public IActionResult Edit(string id, IFormCollection collection)
    {
        var instance = Repository.GetByName(id);
        if (instance == null) return RedirectToAction("Index");

        instance.InstanceProperties.Clear();

        foreach (string key in collection.Keys)
        {
            if (key.Contains("InstancePropertyKey") && collection[key].ToString().Length > 0)
            {
                var propIdx = key.Replace("InstancePropertyKey-", "");
                instance.InstanceProperties.Add(new InstancePropertyModel
                {
                    ParentInstance = instance,
                    PropertyName = collection[key].ToString(),
                    PropertyValue = collection["InstancePropertyValue-" + propIdx].ToString()
                });
            }
        }
        Repository.Save(instance);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(string id)
    {
        var instance = Repository.GetByName(id);
        return View(instance);
    }

    [HttpPost]
    public IActionResult Delete(string id, IFormCollection collection)
    {
        var instance = Repository.GetByName(id);
        if (instance != null) Repository.Delete(instance);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Connect(string id)
    {
        var instance = Repository.GetByName(id);
        if (instance == null) return View("NotFound");

        var ivm = new InstanceViewModel { Instance = instance };
        var sched = await instance.GetQuartzScheduler();
        if (sched == null) return View("NotFound");

        ivm.Jobs = await instance.GetAllJobs();
        ivm.Triggers = await instance.GetAllTriggers();
        return View(ivm);
    }

    public IActionResult WhatIsMyInstanceID(string id)
    {
        var instance = Repository.GetByName(id);
        ViewData["MyTime"] = DateTime.Now.ToString("HH:mm");
        return View(instance);
    }
}
