using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace QuartzAdmin.web.Controllers;

[Route("[controller]/[action]")]
public class AccountController : Controller
{
    public AccountController(IFormsAuthentication formsAuth, IMembershipService service)
    {
        FormsAuth = formsAuth;
        MembershipService = service;
    }

    public IFormsAuthentication FormsAuth { get; private set; }
    public IMembershipService MembershipService { get; private set; }

    public IActionResult LogOn() => View();

    [HttpPost]
    public async Task<IActionResult> LogOn(string userName, string password, bool rememberMe, string? returnUrl)
    {
        if (!ValidateLogOn(userName, password))
            return View();

        await FormsAuth.SignIn(HttpContext, userName, rememberMe);
        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> LogOff()
    {
        await FormsAuth.SignOut(HttpContext);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string userName, string email, string password, string confirmPassword)
    {
        ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

        if (ValidateRegistration(userName, email, password, confirmPassword))
        {
            var createStatus = MembershipService.CreateUser(userName, password, email);
            if (createStatus == MembershipCreateStatus.Success)
            {
                await FormsAuth.SignIn(HttpContext, userName, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
            }
        }
        return View();
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult ChangePassword()
    {
        ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
        return View();
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost]
    public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

        if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            return View();

        try
        {
            if (MembershipService.ChangePassword(User.Identity?.Name ?? "", currentPassword, newPassword))
                return RedirectToAction("ChangePasswordSuccess");

            ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
            return View();
        }
        catch
        {
            ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
            return View();
        }
    }

    public IActionResult ChangePasswordSuccess() => View();

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            throw new InvalidOperationException("Windows authentication is not supported.");
    }

    #region Validation Methods

    private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrEmpty(currentPassword))
            ModelState.AddModelError("currentPassword", "You must specify a current password.");

        if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            ModelState.AddModelError("newPassword",
                string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "You must specify a new password of {0} or more characters.",
                    MembershipService.MinPasswordLength));

        if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");

        return ModelState.IsValid;
    }

    private bool ValidateLogOn(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName))
            ModelState.AddModelError("username", "You must specify a username.");

        if (string.IsNullOrEmpty(password))
            ModelState.AddModelError("password", "You must specify a password.");

        if (!MembershipService.ValidateUser(userName, password))
            ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");

        return ModelState.IsValid;
    }

    private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(userName))
            ModelState.AddModelError("username", "You must specify a username.");

        if (string.IsNullOrEmpty(email))
            ModelState.AddModelError("email", "You must specify an email address.");

        if (password == null || password.Length < MembershipService.MinPasswordLength)
            ModelState.AddModelError("password",
                string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "You must specify a password of {0} or more characters.",
                    MembershipService.MinPasswordLength));

        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");

        return ModelState.IsValid;
    }

    private static string ErrorCodeToString(MembershipCreateStatus createStatus) => createStatus switch
    {
        MembershipCreateStatus.DuplicateUserName => "Username already exists. Please enter a different user name.",
        MembershipCreateStatus.DuplicateEmail => "A username for that e-mail address already exists. Please enter a different e-mail address.",
        MembershipCreateStatus.InvalidPassword => "The password provided is invalid. Please enter a valid password value.",
        MembershipCreateStatus.InvalidEmail => "The e-mail address provided is invalid. Please check the value and try again.",
        MembershipCreateStatus.InvalidAnswer => "The password retrieval answer provided is invalid. Please check the value and try again.",
        MembershipCreateStatus.InvalidQuestion => "The password retrieval question provided is invalid. Please check the value and try again.",
        MembershipCreateStatus.InvalidUserName => "The user name provided is invalid. Please check the value and try again.",
        MembershipCreateStatus.ProviderError => "The authentication provider returned an error. Please verify your entry and try again.",
        MembershipCreateStatus.UserRejected => "The user creation request has been canceled. Please verify your entry and try again.",
        _ => "An unknown error occurred. Please verify your entry and try again."
    };

    #endregion
}

public enum MembershipCreateStatus
{
    Success,
    InvalidUserName,
    InvalidPassword,
    InvalidEmail,
    InvalidAnswer,
    InvalidQuestion,
    DuplicateUserName,
    DuplicateEmail,
    DuplicateProviderUserKey,
    InvalidProviderUserKey,
    UserRejected,
    ProviderError
}

public interface IFormsAuthentication
{
    Task SignIn(HttpContext context, string userName, bool createPersistentCookie);
    Task SignOut(HttpContext context);
}

public class FormsAuthenticationService : IFormsAuthentication
{
    public async Task SignIn(HttpContext context, string userName, bool createPersistentCookie)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };
        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);
        await context.SignInAsync("Cookies", principal, new AuthenticationProperties { IsPersistent = createPersistentCookie });
    }

    public async Task SignOut(HttpContext context)
    {
        await context.SignOutAsync("Cookies");
    }
}

public interface IMembershipService
{
    int MinPasswordLength { get; }
    bool ValidateUser(string userName, string password);
    MembershipCreateStatus CreateUser(string userName, string password, string email);
    bool ChangePassword(string userName, string oldPassword, string newPassword);
}

public class AccountMembershipService : IMembershipService
{
    private readonly Dictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase);

    public int MinPasswordLength => 6;

    public bool ValidateUser(string userName, string password)
    {
        return _users.TryGetValue(userName, out var storedPassword) && storedPassword == password;
    }

    public MembershipCreateStatus CreateUser(string userName, string password, string email)
    {
        if (_users.ContainsKey(userName))
            return MembershipCreateStatus.DuplicateUserName;
        _users[userName] = password;
        return MembershipCreateStatus.Success;
    }

    public bool ChangePassword(string userName, string oldPassword, string newPassword)
    {
        if (!ValidateUser(userName, oldPassword)) return false;
        _users[userName] = newPassword;
        return true;
    }
}
