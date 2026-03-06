using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Fakes;

public class FakeConnectionRepository : IConnectionRepository
{
    private readonly List<ConnectionModel> _connectionList = new();

    public int Count => _connectionList.Count;

    public void AddConnection(ConnectionModel connection)
    {
        _connectionList.Add(connection);
    }

    public ConnectionModel? GetConnection(int connectionId)
    {
        return _connectionList.FirstOrDefault(c => c.ConnectionId == connectionId);
    }

    public bool IsValid(ConnectionModel connection, out IEnumerable<RuleViolation> ruleViolations)
    {
        var ruleViolationList = new List<RuleViolation>();
        var duplicateName = _connectionList.FirstOrDefault(c => c.Name == connection.Name && c.ConnectionId != connection.ConnectionId);
        if (duplicateName != null)
            ruleViolationList.Add(new RuleViolation("Name is already in use", "Name"));
        ruleViolations = ruleViolationList;
        return ruleViolationList.Count == 0;
    }

    public void RemoveConnection(ConnectionModel connection)
    {
        _connectionList.Remove(connection);
    }

    public IEnumerable<ConnectionModel> GetConnections()
    {
        return _connectionList;
    }

    public void Save()
    {
        var duplicates = _connectionList
            .GroupBy(c => c.Name)
            .Where(g => g.Count() > 1)
            .ToList();
        if (duplicates.Any())
            throw new InvalidOperationException("Duplicate connection names are not allowed.");
    }
}
