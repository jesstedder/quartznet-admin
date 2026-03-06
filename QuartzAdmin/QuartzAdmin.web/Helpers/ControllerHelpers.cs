using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Helpers;

public static class ControllerHelpers
{
    public static void AddRuleViolations(this ModelStateDictionary modelState, IEnumerable<RuleViolation> ruleViolations)
    {
        foreach (var ruleViolation in ruleViolations)
        {
            modelState.AddModelError(ruleViolation.PropertyName ?? Guid.NewGuid().ToString(), ruleViolation.ErrorMessage);
        }
    }
}
