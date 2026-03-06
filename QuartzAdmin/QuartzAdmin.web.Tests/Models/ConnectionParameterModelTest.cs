using Microsoft.AspNetCore.Http;
using Xunit;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Models;

public class ConnectionParameterModelTest
{
    private static IFormCollection CreateFormCollection(Dictionary<string, string> values)
    {
        var dict = values.ToDictionary(
            kvp => kvp.Key,
            kvp => new Microsoft.Extensions.Primitives.StringValues(kvp.Value));
        return new FormCollection(dict);
    }

    [Fact]
    public void Should_Instantiate()
    {
        var connectionParameter = new ConnectionParameterModel();
        Assert.NotNull(connectionParameter);
    }

    [Fact]
    public void Should_Create_List_From_Valid_Form_Collection()
    {
        var formCollection = CreateFormCollection(new Dictionary<string, string>
        {
            ["ConnectionParameterKey1"] = "key1",
            ["ConnectionParameterValue1"] = "value1"
        });

        var connectionParameterList = ConnectionParameterModel.FromFormCollection(formCollection);

        Assert.Single(connectionParameterList);
        Assert.Equal("key1", connectionParameterList[0].Key);
        Assert.Equal("value1", connectionParameterList[0].Value);
    }

    [Fact]
    public void Should_Create_Empty_List_From_Empty_Form_Collection()
    {
        var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
        var connectionParameterList = ConnectionParameterModel.FromFormCollection(formCollection);
        Assert.Empty(connectionParameterList);
    }

    [Fact]
    public void Should_Create_List_From_Form_Collection_Without_Value()
    {
        var formCollection = CreateFormCollection(new Dictionary<string, string>
        {
            ["ConnectionParameterKey1"] = "key1"
        });

        var connectionParameterList = ConnectionParameterModel.FromFormCollection(formCollection);

        Assert.Single(connectionParameterList);
        Assert.Equal("key1", connectionParameterList[0].Key);
        Assert.False(connectionParameterList[0].IsValid);
    }
}
