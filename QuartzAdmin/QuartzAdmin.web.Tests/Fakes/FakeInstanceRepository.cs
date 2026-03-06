using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Fakes;

public class FakeInstanceRepository : IInstanceRepository
{
    private readonly List<InstanceModel> _instances = new();

    public void Delete(InstanceModel instance)
    {
        _instances.Remove(instance);
    }

    public void Save(InstanceModel instance)
    {
        var found = _instances.Find(x => x.InstanceName == instance.InstanceName);
        if (found != null)
            _instances.Remove(found);
        _instances.Add(instance);
    }

    public InstanceModel? GetByName(string name)
    {
        return _instances.FirstOrDefault(x => x.InstanceName == name);
    }

    public InstanceModel? GetInstance(string instanceName) => GetByName(instanceName);

    public List<InstanceModel> GetAll()
    {
        return _instances.ToList();
    }
}
