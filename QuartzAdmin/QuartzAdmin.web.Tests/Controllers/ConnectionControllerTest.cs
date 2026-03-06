using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Xunit;
using QuartzAdmin.web.Controllers;
using QuartzAdmin.web.Models;
using QuartzAdmin.web.Tests.Fakes;

namespace QuartzAdmin.web.Tests.Controllers;

public class ConnectionControllerTest
{
    private static IFormCollection CreateFormCollection(Dictionary<string, string> values)
    {
        var dict = values.ToDictionary(k => k.Key, v => new StringValues(v.Value));
        return new FormCollection(dict);
    }

    private static T SetupController<T>(T controller) where T : Controller
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public void Create_Get_Should_Return_View()
    {
        var connectionController = SetupController(new ConnectionController(new FakeConnectionRepository()));
        var viewResult = connectionController.Create() as ViewResult;
        Assert.NotNull(viewResult);
    }

    [Fact]
    public void Create_Post_Should_Create_Connection_When_Valid()
    {
        var connectionRepository = new FakeConnectionRepository();
        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var collection = CreateFormCollection(new Dictionary<string, string>
        {
            ["Name"] = "connection1",
            ["ConnectionParameterKey1"] = "key1",
            ["ConnectionParameterValue1"] = "value1"
        });

        connectionController.Create(collection);
        Assert.Equal(1, connectionRepository.Count);
    }

    [Fact]
    public void Create_Post_Should_Not_Create_Connection_When_Invalid()
    {
        var connectionRepository = new FakeConnectionRepository();
        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var collection = new FormCollection(new Dictionary<string, StringValues>());

        connectionController.Create(collection);
        Assert.Equal(0, connectionRepository.Count);
    }

    [Fact]
    public void Create_Post_Should_Redisplay_With_Errors_When_Invalid()
    {
        var connectionRepository = new FakeConnectionRepository();
        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var collection = new FormCollection(new Dictionary<string, StringValues>());

        var viewResult = connectionController.Create(collection) as ViewResult;

        Assert.NotNull(viewResult);
        Assert.IsType<ConnectionModel>(viewResult.Model);
        Assert.True(viewResult.ViewData.ModelState["Name"]?.Errors.Count > 0);
    }

    [Fact]
    public void Create_Post_Should_Redisplay_With_Errors_When_Duplicate_Name()
    {
        var connectionRepository = new FakeConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 1, Name = "connection1" });

        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var collection = CreateFormCollection(new Dictionary<string, string>
        {
            ["Name"] = "connection1",
            ["ConnectionParameterKey1"] = "key1",
            ["ConnectionParameterValue1"] = "value1"
        });

        var viewResult = connectionController.Create(collection) as ViewResult;
        Assert.True(viewResult?.ViewData.ModelState["Name"]?.Errors.Count == 1);
    }

    [Fact]
    public void Index_Get_Should_Return_View()
    {
        var connectionRepository = new FakeConnectionRepository();
        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var viewResult = connectionController.Index() as ViewResult;
        Assert.NotNull(viewResult);
    }

    [Fact]
    public void Index_Get_Should_Display_Connections()
    {
        var connectionRepository = new FakeConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 1, Name = "connection1" });
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 2, Name = "connection2" });

        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var viewResult = connectionController.Index() as ViewResult;
        var connectionList = viewResult?.Model as IEnumerable<ConnectionModel>;

        Assert.NotNull(connectionList);
        Assert.Equal(2, connectionList.Count());
    }

    [Fact]
    public void Edit_Get_Should_Return_View_When_Valid_Id()
    {
        var connectionRepository = new FakeConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 1, Name = "connection1" });

        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var viewResult = connectionController.Edit(1) as ViewResult;

        Assert.NotNull(viewResult);
    }

    [Fact]
    public void Edit_Get_Should_Display_Connection_When_Valid_Id()
    {
        var connectionRepository = new FakeConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 1, Name = "connection1" });

        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var viewResult = connectionController.Edit(1) as ViewResult;
        var connection = viewResult?.Model as ConnectionModel;

        Assert.NotNull(connection);
        Assert.Equal(1, connection.ConnectionId);
    }

    [Fact]
    public void Edit_Get_Should_Display_Error_When_Invalid_Id()
    {
        var connectionRepository = new FakeConnectionRepository();
        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var viewResult = connectionController.Edit(1) as ViewResult;

        Assert.NotNull(viewResult);
        Assert.Equal("NotFound", viewResult.ViewName);
    }

    [Fact]
    public void Edit_Post_Should_Update_Connection_When_Valid()
    {
        var connectionRepository = new FakeConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 1, Name = "connection1" });

        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var collection = CreateFormCollection(new Dictionary<string, string>
        {
            ["Name"] = "connectionA",
            ["ConnectionParameterKey1"] = "key1",
            ["ConnectionParameterValue1"] = "value1"
        });

        connectionController.Edit(1, collection);
        var connection = connectionRepository.GetConnection(1);

        Assert.Equal("connectionA", connection?.Name);
    }

    [Fact]
    public void Edit_Post_Should_Redisplay_With_Errors_When_Invalid()
    {
        var connectionRepository = new FakeConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 1, Name = "connection1" });

        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var collection = CreateFormCollection(new Dictionary<string, string> { ["Name"] = "" });

        var viewResult = connectionController.Edit(1, collection) as ViewResult;

        Assert.NotNull(viewResult);
        Assert.IsType<ConnectionModel>(viewResult.Model);
        Assert.True(viewResult.ViewData.ModelState["Name"]?.Errors.Count > 0);
    }

    [Fact]
    public void Edit_Post_Should_Redisplay_With_Errors_When_Duplicate_Name()
    {
        var connectionRepository = new FakeConnectionRepository();
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 1, Name = "connection1" });
        connectionRepository.AddConnection(new ConnectionModel { ConnectionId = 2, Name = "connection2" });

        var connectionController = SetupController(new ConnectionController(connectionRepository));
        var collection = CreateFormCollection(new Dictionary<string, string>
        {
            ["Name"] = "connection2",
            ["ConnectionParameterKey1"] = "key1",
            ["ConnectionParameterValue1"] = "value1"
        });

        var viewResult = connectionController.Edit(1, collection) as ViewResult;
        Assert.True(viewResult?.ViewData.ModelState["Name"]?.Errors.Count == 1);
    }
}
