using Xunit;
using QuartzAdmin.web.Tests.Fakes;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Models;

public class ConnectionRepositoryTest
{
    private static IConnectionRepository GetConnectionRepository() => new FakeConnectionRepository();

    [Fact]
    public void Should_Instantiate()
    {
        var connectionRepository = GetConnectionRepository();
        Assert.NotNull(connectionRepository);
    }

    [Fact]
    public void Should_Have_Count()
    {
        var connectionRepository = GetConnectionRepository();
        Assert.Equal(0, connectionRepository.Count);
    }

    [Fact]
    public void Should_Allow_Add()
    {
        var connectionRepository = GetConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel());
        Assert.Equal(1, connectionRepository.Count);
    }

    [Fact]
    public void Should_Return_Item_For_Valid_Id()
    {
        var connectionRepository = GetConnectionRepository();
        var connection = new ConnectionModel { ConnectionId = 1 };
        connectionRepository.AddConnection(connection);

        var retrievedConnection = connectionRepository.GetConnection(1);
        Assert.Equal(1, retrievedConnection?.ConnectionId);
    }

    [Fact]
    public void Should_Return_Null_For_Invalid_Id()
    {
        var connectionRepository = GetConnectionRepository();
        var retrievedConnection = connectionRepository.GetConnection(1);
        Assert.Null(retrievedConnection);
    }

    [Fact]
    public void Should_Allow_Remove()
    {
        var connectionRepository = GetConnectionRepository();
        var connection = new ConnectionModel { ConnectionId = 1 };
        connectionRepository.AddConnection(connection);

        connectionRepository.RemoveConnection(connection);
        Assert.Null(connectionRepository.GetConnection(1));
    }

    [Fact]
    public void Should_Not_Save_When_Duplicate_Name()
    {
        var connectionRepository = GetConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { Name = "connection1" });
        connectionRepository.AddConnection(new ConnectionModel { Name = "connection1" });

        Assert.Throws<InvalidOperationException>(() => connectionRepository.Save());
    }
}
