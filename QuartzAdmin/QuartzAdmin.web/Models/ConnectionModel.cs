namespace QuartzAdmin.web.Models;

public class ConnectionModel : IValidatingModel
{
    public int ConnectionId { get; set; }
    public string? Name { get; set; }

    private List<ConnectionParameterModel> _connectionParameters = new();
    public List<ConnectionParameterModel> ConnectionParameters => _connectionParameters;

    public bool IsValid => !GetRuleViolations().Any();

    public IEnumerable<RuleViolation> GetRuleViolations()
    {
        if (string.IsNullOrEmpty(Name))
            yield return new RuleViolation("Name required", "Name");

        if (_connectionParameters.Count == 0)
            yield return new RuleViolation("At least one connection parameter required");

        foreach (var param in _connectionParameters)
            foreach (var violation in param.GetRuleViolations())
                yield return violation;
    }
}
