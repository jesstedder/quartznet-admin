using Xunit;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Models;

public class ConnectionModelTest
{
    private static ConnectionModel CreateValidConnection()
    {
        var connection = new ConnectionModel();
        connection.Name = "name";
        connection.ConnectionParameters.Add(new ConnectionParameterModel { Key = "key", Value = "value" });
        return connection;
    }

    [Fact]
    public void Should_Instantiate()
    {
        var connection = new ConnectionModel();
        Assert.NotNull(connection);
    }

    [Fact]
    public void Should_Not_Be_Valid_When_Unitialized()
    {
        var connection = new ConnectionModel();
        Assert.False(connection.IsValid);
    }

    [Fact]
    public void Should_Be_Valid_When_Initialized()
    {
        var connection = CreateValidConnection();
        Assert.True(connection.IsValid);
    }

    [Fact]
    public void Should_Not_Be_Valid_When_Name_Null()
    {
        var connection = CreateValidConnection();
        connection.Name = null;
        Assert.False(connection.IsValid);
    }

    [Fact]
    public void Should_Not_Be_Valid_When_Name_Empty()
    {
        var connection = CreateValidConnection();
        connection.Name = string.Empty;
        Assert.False(connection.IsValid);
    }

    [Fact]
    public void Should_Not_Be_Valid_When_Zero_Parameters()
    {
        var connection = CreateValidConnection();
        connection.ConnectionParameters.Clear();
        Assert.False(connection.IsValid);
    }

    [Fact]
    public void Should_Not_Be_Valid_When_Parameters_Not_Valid()
    {
        var connection = CreateValidConnection();
        connection.ConnectionParameters.Add(new ConnectionParameterModel { Key = "key", Value = null });
        Assert.False(connection.IsValid);
    }
}
