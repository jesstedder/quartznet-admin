namespace QuartzAdmin.web.Models;

public interface IValidatingModel
{
    bool IsValid { get; }
    IEnumerable<RuleViolation> GetRuleViolations();
}
