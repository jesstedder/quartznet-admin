using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using QuartzAdmin.web.Controllers;

namespace QuartzAdmin.web.Tests.Controllers;

public class AccountControllerTest
{
    [Fact]
    public void LogOn_Get_Returns_View()
    {
        var controller = new AccountController(new FormsAuthenticationService(), new AccountMembershipService());
        var result = controller.LogOn() as ViewResult;
        Assert.NotNull(result);
    }

    [Fact]
    public void Register_Get_Returns_View_With_PasswordLength()
    {
        var mockMembership = new Mock<IMembershipService>();
        mockMembership.Setup(m => m.MinPasswordLength).Returns(8);

        var controller = new AccountController(new FormsAuthenticationService(), mockMembership.Object);
        var result = controller.Register() as ViewResult;

        Assert.NotNull(result);
        Assert.Equal(8, result.ViewData["PasswordLength"]);
    }

    [Fact]
    public void ChangePasswordSuccess_Returns_View()
    {
        var controller = new AccountController(new FormsAuthenticationService(), new AccountMembershipService());
        var result = controller.ChangePasswordSuccess() as ViewResult;
        Assert.NotNull(result);
    }

    [Fact]
    public void ValidateUser_Returns_False_For_Invalid_Credentials()
    {
        var service = new AccountMembershipService();
        Assert.False(service.ValidateUser("admin", "wrongpassword"));
    }

    [Fact]
    public void CreateUser_Returns_Success()
    {
        var service = new AccountMembershipService();
        var status = service.CreateUser("newuser", "password123", "email@test.com");
        Assert.Equal(MembershipCreateStatus.Success, status);
    }

    [Fact]
    public void CreateUser_Returns_DuplicateUserName_When_User_Exists()
    {
        var service = new AccountMembershipService();
        service.CreateUser("existinguser", "password123", "email@test.com");
        var status = service.CreateUser("existinguser", "password456", "other@test.com");
        Assert.Equal(MembershipCreateStatus.DuplicateUserName, status);
    }
}
