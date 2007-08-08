using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.IdentityModel.Selectors;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using ProjectTracker.Library.Security;

namespace PTWcfServiceAuth
{
  /// <summary>
  /// Custom authorization policy for WCF.
  /// </summary>
  /// <remarks>
  /// This policy sets the current principal and identity
  /// to the values provided earlier in the process
  /// by the CredentialValidator class.
  /// </remarks>
  public class PrincipalPolicy : IAuthorizationPolicy
  {
    string id = Guid.NewGuid().ToString();

    public string Id
    {
      get { return this.id; }
    }

    public ClaimSet Issuer
    {
      get { return ClaimSet.System; }
    }

    public bool Evaluate(EvaluationContext context, ref object state)
    {
      // get the identities list from the context
      object obj;
      if (!context.Properties.TryGetValue("Identities", out obj))
        return false;
      IList<IIdentity> identities = obj as IList<IIdentity>;

      // make sure there is already a default identity
      if (identities == null || identities.Count <= 0)
        return false;

      // load prinicpal based on username authenticated in CredentialValidator
      string username = identities[0].Name;
      PTPrincipal.Logout();
      if (username != "anonymous")
        PTPrincipal.LoadPrincipal(username);
      IPrincipal principal = Csla.ApplicationContext.User;

      // tell WCF to use the custom principal 
      context.Properties["Principal"] = principal;
      
      // tell WCF to use the custom identity 
      identities[0] = principal.Identity;

      return true;
    }
  }
}
