using Microsoft.EntityFrameworkCore;

namespace QuartzAdmin.web.Models;

public class InstanceRepository : IInstanceRepository
{
    private readonly AppDbContext _context;

    public InstanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<InstanceModel> GetAll()
    {
        return _context.Instances.Include(i => i.InstanceProperties).ToList();
    }

    public InstanceModel? GetByName(string instanceName)
    {
        return _context.Instances
            .Include(i => i.InstanceProperties)
            .FirstOrDefault(i => i.InstanceName == instanceName);
    }

    public InstanceModel? GetInstance(string instanceName) => GetByName(instanceName);

    public void Save(InstanceModel instance)
    {
        var existing = _context.Instances
            .Include(i => i.InstanceProperties)
            .FirstOrDefault(i => i.InstanceID == instance.InstanceID);

        if (existing == null)
        {
            _context.Instances.Add(instance);
        }
        _context.SaveChanges();
    }

    public void Delete(InstanceModel instance)
    {
        _context.Instances.Remove(instance);
        _context.SaveChanges();
    }
}
