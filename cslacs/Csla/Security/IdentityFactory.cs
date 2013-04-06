#if !CLIENTONLY
using System;
using System.Web.Security;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// IdentityFactory is an object in charge of retrieving <see cref="MembershipIdentity.Criteria"/> 
  /// on the web server and transferring it back to the client.
  /// </summary>
  [Serializable]
  public sealed class IdentityFactory : MembershipIdentity
  {
    /// <summary>
    /// Fetches MembershipIdentity from the server
    /// </summary>
    /// <param name="criteria"><see cref="MembershipIdentity.Criteria"/></param>
    /// <returns></returns>
    public MembershipIdentity FetchMembershipIdentity(MembershipIdentity.Criteria criteria)
    {
      if (criteria.IsRunOnWebServer)
        return GetIdentityOnServer(criteria);
      else
      {
        IdentityFactory serverIdentity = DataPortal.Fetch<IdentityFactory>(criteria);
        Type identityType = Type.GetType(criteria.MembershipIdentityType);
        return GetIdentity(identityType, serverIdentity);
      }
    }

    internal MembershipIdentity GetIdentityOnServer(MembershipIdentity.Criteria criteria)
    {
      Type identityType = Type.GetType(criteria.MembershipIdentityType);
      
      var returnValue = (MembershipIdentity)Activator.CreateInstance(identityType, true);
      
      if (Membership.ValidateUser(criteria.Name, criteria.Password))
      {
        returnValue.IsAuthenticated = true;
        returnValue.Name = criteria.Name;
        returnValue.Roles = new MobileList<string>(System.Web.Security.Roles.GetRolesForUser(criteria.Name));
        returnValue.AuthenticationType = "Csla";
        returnValue.LoadCustomData();
      }
      else
      {
        returnValue.IsAuthenticated = false;
        returnValue.Name = string.Empty;
        returnValue.Roles = new MobileList<string>();
        returnValue.AuthenticationType = "";
      }
      return returnValue;
    }

    private MembershipIdentity GetIdentity(Type identityType, IdentityFactory identity)
    {
      MembershipIdentity returnValue = (MembershipIdentity)Activator.CreateInstance(identityType);
      returnValue.IsAuthenticated = identity.IsAuthenticated;
      returnValue.Name = identity.Name;
      returnValue.Roles = identity.Roles;
      returnValue.AuthenticationType = "Csla";
      return returnValue;
    }

    private void DataPortal_Fetch(MembershipIdentity.Criteria criteria)
    {
      MembershipIdentity target = GetIdentityOnServer(criteria);
      IsAuthenticated = target.IsAuthenticated;
      Name = target.Name;
      Roles = target.Roles;
      AuthenticationType = "Csla";
    }
  }
}
#endif