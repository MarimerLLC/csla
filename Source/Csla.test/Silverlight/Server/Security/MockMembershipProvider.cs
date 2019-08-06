//-----------------------------------------------------------------------
// <copyright file="MockMembershipProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Mock implementation of a MembershipProvider that gives us predetermined responses for our tests</summary>
//-----------------------------------------------------------------------
using System;
using System.Web.Security;

namespace Csla.Testing.Business.Security
{
  /// <summary>
  /// Mock implementation of a MembershipProvider that gives us predetermined responses for our tests
  /// </summary>
  public class MockMembershipProvider : MembershipProvider
  {
    public override bool ValidateUser(string username, string password)
    {
      return (username == "user" && password == "1234") ||
        (username == "admin" && password == "12345");
    }

    string _ApplicationName;
    public override string ApplicationName
    {
      get
      {
        return _ApplicationName;
      }
      set
      {
        _ApplicationName = value;
      }
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      throw new NotImplementedException();
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      throw new NotImplementedException();
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      throw new NotImplementedException();
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      throw new NotImplementedException();
    }

    public override bool EnablePasswordReset
    {
      get { throw new NotImplementedException(); }
    }

    public override bool EnablePasswordRetrieval
    {
      get { throw new NotImplementedException(); }
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotImplementedException();
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotImplementedException();
    }

    public override int GetNumberOfUsersOnline()
    {
      throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer)
    {
      throw new NotImplementedException();
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      throw new NotImplementedException();
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      throw new NotImplementedException();
    }

    public override string GetUserNameByEmail(string email)
    {
      throw new NotImplementedException();
    }

    public override int MaxInvalidPasswordAttempts
    {
      get { throw new NotImplementedException(); }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
      get { throw new NotImplementedException(); }
    }

    public override int MinRequiredPasswordLength
    {
      get { throw new NotImplementedException(); }
    }

    public override int PasswordAttemptWindow
    {
      get { throw new NotImplementedException(); }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
      get { throw new NotImplementedException(); }
    }

    public override string PasswordStrengthRegularExpression
    {
      get { throw new NotImplementedException(); }
    }

    public override bool RequiresQuestionAndAnswer
    {
      get { throw new NotImplementedException(); }
    }

    public override bool RequiresUniqueEmail
    {
      get { throw new NotImplementedException(); }
    }

    public override string ResetPassword(string username, string answer)
    {
      throw new NotImplementedException();
    }

    public override bool UnlockUser(string userName)
    {
      throw new NotImplementedException();
    }

    public override void UpdateUser(MembershipUser user)
    {
      throw new NotImplementedException();
    }
  }

}