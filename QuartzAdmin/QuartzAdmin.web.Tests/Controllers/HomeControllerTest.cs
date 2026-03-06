using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using QuartzAdmin.web.Controllers;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Controllers;

public class HomeControllerTest
{
    [Fact]
    public void Index_Returns_View_With_Instances()
    {
        var mockRepo = new Mock<IInstanceRepository>();
        mockRepo.Setup(r => r.GetAll()).Returns(new List<InstanceModel>
        {
            new InstanceModel { InstanceName = "Instance1" }
        });

        var controller = new HomeController(mockRepo.Object);
        var result = controller.Index() as ViewResult;

        Assert.NotNull(result);
        var model = result.Model as List<InstanceModel>;
        Assert.NotNull(model);
        Assert.Single(model);
    }

    [Fact]
    public void About_Returns_View()
    {
        var mockRepo = new Mock<IInstanceRepository>();
        var controller = new HomeController(mockRepo.Object);
        var result = controller.About() as ViewResult;
        Assert.NotNull(result);
    }
}
