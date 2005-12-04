using System;
using ProjectTracker.Library.Security;
using System.Web.Security;

public class PTMembershipProvider : MembershipProvider
{

  public override bool ValidateUser(string username, string password)
  {
    PTPrincipal.Login(username, password);
    System.Web.HttpContext.Current.User = System.Threading.Thread.CurrentPrincipal;
    if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
    {
      System.Web.HttpContext.Current.Session["CslaPrincipal"] = System.Threading.Thread.CurrentPrincipal;
      return true;
    }
    return false;
  }

  #region Non-Implemented Members

  // the following members must be implemented due to the abstract class MembershipProvider,
  // but not required to be functional for using the Login control.
  public override bool ChangePassword(string username, string oldPassword, string newPassword)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override bool DeleteUser(string username, bool deleteAllRelatedData)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override bool EnablePasswordReset
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override bool EnablePasswordRetrieval
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override int GetNumberOfUsersOnline()
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override string GetPassword(string username, string answer)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override MembershipUser GetUser(string username, bool userIsOnline)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override string GetUserNameByEmail(string email)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override int MaxInvalidPasswordAttempts
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override int MinRequiredNonAlphanumericCharacters
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override int MinRequiredPasswordLength
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override int PasswordAttemptWindow
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override MembershipPasswordFormat PasswordFormat
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override string PasswordStrengthRegularExpression
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override bool RequiresQuestionAndAnswer
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override bool RequiresUniqueEmail
  {
    get { throw new Exception("The method or operation is not implemented."); }
  }

  public override string ResetPassword(string username, string answer)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override bool UnlockUser(string userName)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override void UpdateUser(MembershipUser user)
  {
    throw new Exception("The method or operation is not implemented.");
  }

  public override string ApplicationName
  {
    get
    {
      throw new Exception("The method or operation is not implemented.");
    }
    set
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }

  #endregion

}

