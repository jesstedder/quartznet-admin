using Xunit;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Models;

public class InstanceModelTest
{
    [Fact]
    public void Should_Instantiate()
    {
        var instance = new InstanceModel();
        Assert.NotNull(instance);
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Name_Is_Null()
    {
        var instance = new InstanceModel();
        instance.InstanceName = null!;
        Assert.False(instance.IsValid());
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Name_Is_Empty()
    {
        var instance = new InstanceModel();
        instance.InstanceName = string.Empty;
        Assert.False(instance.IsValid());
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Properties_Are_Empty()
    {
        var instance = new InstanceModel();
        instance.InstanceName = "TestInstance";
        Assert.False(instance.IsValid());
    }

    [Fact]
    public void IsValid_Should_Return_True_When_Name_And_Properties_Are_Set()
    {
        var instance = new InstanceModel();
        instance.InstanceName = "TestInstance";
        instance.InstanceProperties.Add(new InstancePropertyModel
        {
            PropertyName = "quartz.scheduler.instanceName",
            PropertyValue = "TestScheduler"
        });
        Assert.True(instance.IsValid());
    }

    [Fact]
    public void Should_Add_And_Clear_Properties()
    {
        var instance = new InstanceModel();
        instance.InstanceProperties.Add(new InstancePropertyModel { PropertyName = "key", PropertyValue = "value" });
        Assert.Single(instance.InstanceProperties);

        instance.InstanceProperties.Clear();
        Assert.Empty(instance.InstanceProperties);
    }
}
