using Microsoft.AspNetCore.Http;

namespace QuartzAdmin.web.Models;

public class ConnectionParameterModel : IValidatingModel
{
    public string? Key { get; set; }
    public string? Value { get; set; }

    public bool IsValid => !GetRuleViolations().Any();

    public IEnumerable<RuleViolation> GetRuleViolations()
    {
        if (string.IsNullOrEmpty(Key))
            yield return new RuleViolation("Parameter key required", "Key");

        if (string.IsNullOrEmpty(Value))
            yield return new RuleViolation("Parameter value required", "Value");
    }

    public static List<ConnectionParameterModel> FromFormCollection(IFormCollection formCollection)
    {
        var connectionParameterList = new List<ConnectionParameterModel>();
        const string keyPrefix = "ConnectionParameterKey";
        const string valuePrefix = "ConnectionParameterValue";

        foreach (string key in formCollection.Keys)
        {
            if (key.StartsWith(keyPrefix))
            {
                var parameterIndex = key.Remove(0, keyPrefix.Length);
                var parameterKey = formCollection[key].ToString();
                var parameterValue = formCollection[valuePrefix + parameterIndex].ToString();
                connectionParameterList.Add(new ConnectionParameterModel { Key = parameterKey, Value = parameterValue });
            }
        }
        return connectionParameterList;
    }
}
