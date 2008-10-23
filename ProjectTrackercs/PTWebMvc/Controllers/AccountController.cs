using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace PTWebMvc.Controllers
{

  [HandleError]
  [OutputCache(Location = OutputCacheLocation.None)]
  public class AccountController : Controller
  {

    public AccountController()
      : this(null, null)
    {
    }

    public AccountController(IFormsAuthentication formsAuth, MembershipProvider provider)
    {
      FormsAuth = formsAuth ?? new FormsAuthenticationWrapper();
      Provider = provider ?? new Security.PTMembershipProvider(); //Membership.Provider;
    }

    public IFormsAuthentication FormsAuth
    {
      get;
      private set;
    }

    public MembershipProvider Provider
    {
      get;
      private set;
    }

    [Authorize]
    public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {

      ViewData["Title"] = "Change Password";
      ViewData["PasswordLength"] = Provider.MinRequiredPasswordLength;

      // Non-POST requests should just display the ChangePassword form 
      if (Request.HttpMethod != "POST")
      {
        return View();
      }

      // Basic parameter validation
      List<string> errors = new List<string>();

      if (String.IsNullOrEmpty(currentPassword))
      {
        errors.Add("You must specify a current password.");
      }
      if (newPassword == null || newPassword.Length < Provider.MinRequiredPasswordLength)
      {
        errors.Add(String.Format(CultureInfo.InvariantCulture,
                 "You must specify a new password of {0} or more characters.",
                 Provider.MinRequiredPasswordLength));
      }
      if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
      {
        errors.Add("The new password and confirmation password do not match.");
      }

      if (errors.Count == 0)
      {

        // Attempt to change password
        MembershipUser currentUser = Provider.GetUser(User.Identity.Name, true /* userIsOnline */);
        bool changeSuccessful = false;
        try
        {
          changeSuccessful = currentUser.ChangePassword(currentPassword, newPassword);
        }
        catch
        {
          // An exception is thrown if the new password does not meet the provider's requirements
        }

        if (changeSuccessful)
        {
          return RedirectToAction("ChangePasswordSuccess");
        }
        else
        {
          errors.Add("The current password is incorrect or the new password is invalid.");
        }
      }

      // If we got this far, something failed, redisplay form
      ViewData["errors"] = errors;
      return View();
    }

    public ActionResult ChangePasswordSuccess()
    {

      ViewData["Title"] = "Change Password";

      return View();
    }

    public ActionResult Login(string username, string password, bool? rememberMe)
    {

      ViewData["Title"] = "Login";

      // Non-POST requests should just display the Login form 
      if (Request.HttpMethod != "POST")
      {
        return View();
      }

      // Basic parameter validation
      List<string> errors = new List<string>();

      if (String.IsNullOrEmpty(username))
      {
        errors.Add("You must specify a username.");
      }

      if (errors.Count == 0)
      {

        // Attempt to login
        bool loginSuccessful = Provider.ValidateUser(username, password);

        if (loginSuccessful)
        {

          FormsAuth.SetAuthCookie(username, rememberMe ?? false);
          return RedirectToAction("Index", "Home");
        }
        else
        {
          errors.Add("The username or password provided is incorrect.");
        }
      }

      // If we got this far, something failed, redisplay form
      ViewData["errors"] = errors;
      ViewData["username"] = username;
      return View();
    }

    public ActionResult Logout()
    {

      FormsAuth.SignOut();
      return RedirectToAction("Index", "Home");
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext.HttpContext.User.Identity is WindowsIdentity)
      {
        throw new InvalidOperationException("Windows authentication is not supported.");
      }
    }

    public ActionResult Register(string username, string email, string password, string confirmPassword)
    {

      ViewData["Title"] = "Register";
      ViewData["PasswordLength"] = Provider.MinRequiredPasswordLength;

      // Non-POST requests should just display the Register form 
      if (Request.HttpMethod != "POST")
      {
        return View();
      }

      // Basic parameter validation
      List<string> errors = new List<string>();

      if (String.IsNullOrEmpty(username))
      {
        errors.Add("You must specify a username.");
      }
      if (String.IsNullOrEmpty(email))
      {
        errors.Add("You must specify an email address.");
      }
      if (password == null || password.Length < Provider.MinRequiredPasswordLength)
      {
        errors.Add(String.Format(CultureInfo.InvariantCulture,
                 "You must specify a password of {0} or more characters.",
                 Provider.MinRequiredPasswordLength));
      }
      if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
      {
        errors.Add("The password and confirmation do not match.");
      }

      if (errors.Count == 0)
      {

        // Attempt to register the user
        MembershipCreateStatus createStatus;
        MembershipUser newUser = Provider.CreateUser(username, password, email, null, null, true, null, out createStatus);

        if (newUser != null)
        {

          FormsAuth.SetAuthCookie(username, false /* createPersistentCookie */);
          return RedirectToAction("Index", "Home");
        }
        else
        {
          errors.Add(ErrorCodeToString(createStatus));
        }
      }

      // If we got this far, something failed, redisplay form
      ViewData["errors"] = errors;
      ViewData["username"] = username;
      ViewData["email"] = email;
      return View();
    }

    public static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "Username already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A username for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
  }

  // The FormsAuthentication type is sealed and contains static members, so it is difficult to
  // unit test code that calls its members. The interface and helper class below demonstrate
  // how to create an abstract wrapper around such a type in order to make the AccountController
  // code unit testable.

  public interface IFormsAuthentication
  {
    void SetAuthCookie(string userName, bool createPersistentCookie);
    void SignOut();
  }

  public class FormsAuthenticationWrapper : IFormsAuthentication
  {
    public void SetAuthCookie(string userName, bool createPersistentCookie)
    {
      FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
    }
    public void SignOut()
    {
      FormsAuthentication.SignOut();
    }
  }
}
