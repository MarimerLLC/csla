﻿﻿using System;
using ProjectTracker.Library.Security;
using System.Web;
using System.Web.Security;

namespace Mvc3UI
{
  public class CustomMembershipProvider : MembershipProvider
  {
    public override bool ValidateUser(
      string username, string password)
    {
      bool result = PTPrincipal.Login(username, password);
      return result;
    }

    #region Non-Implemented Members

    // the following members must be implemented due to the abstract class MembershipProvider,
    // but not required to be functional for using the Login control.
    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override bool EnablePasswordReset
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override bool EnablePasswordRetrieval
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override int GetNumberOfUsersOnline()
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override string GetPassword(string username, string answer)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override string GetUserNameByEmail(string email)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override int MaxInvalidPasswordAttempts
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override int MinRequiredPasswordLength
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override int PasswordAttemptWindow
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override string PasswordStrengthRegularExpression
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override bool RequiresQuestionAndAnswer
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override bool RequiresUniqueEmail
    {
      get { throw new NotSupportedException("The method or operation is not implemented."); }
    }

    public override string ResetPassword(string username, string answer)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override bool UnlockUser(string userName)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override void UpdateUser(MembershipUser user)
    {
      throw new NotSupportedException("The method or operation is not implemented.");
    }

    public override string ApplicationName
    {
      get
      {
        throw new NotSupportedException("The method or operation is not implemented.");
      }
      set
      {
        throw new NotSupportedException("The method or operation is not implemented.");
      }
    }

    #endregion
  }
}
