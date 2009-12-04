using System;
using System.Configuration;
using System.Web;
using System.IdentityModel.Selectors;
using System.ServiceModel;
using ProjectTracker.Library.Security;

namespace PTWcfServiceAuth
{
  /// <summary>
  /// Summary description for CredentialValidator
  /// </summary>
  public class CredentialValidator : UserNamePasswordValidator
  {
    public override void Validate(string userName, string password)
    {
      if (userName != "anonymous")
      {
        PTPrincipal.Logout();
        //if (!PTPrincipal.VerifyCredentials(userName, password))
        if (!PTPrincipal.Login(userName, password))
          throw new FaultException("Unknown username or password");

        // add current principal to rolling cache
        Csla.Security.PrincipalCache.AddPrincipal(Csla.ApplicationContext.User);
      }
    }
  }
}