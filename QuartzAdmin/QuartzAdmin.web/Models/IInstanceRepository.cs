namespace QuartzAdmin.web.Models;

public interface IInstanceRepository
{
    void Save(InstanceModel instance);
    void Delete(InstanceModel instance);
    InstanceModel? GetByName(string name);
    InstanceModel? GetInstance(string instanceName);
    List<InstanceModel> GetAll();
}
