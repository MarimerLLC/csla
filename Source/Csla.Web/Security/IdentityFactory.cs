//-----------------------------------------------------------------------
// <copyright file="IdentityFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Verifies user credentials and creates principal/identity based</summary>
//-----------------------------------------------------------------------
using System;
using System.Configuration;
using System.Web.Security;
using Csla.Core;
using Csla.Server;
using Csla.Security;

namespace Csla.Web.Security
{
  /// <summary>
  /// Verifies user credentials and creates principal/identity based
  /// on the ASP.NET Membership provider.
  /// </summary>
  /// <remarks>
  /// If the CslaMembershipUseWebServer config setting is false the
  /// call will be delegated to the app server (if any) behind the
  /// web server. The default is to use the membership provider and
  /// database located on the web server.
  /// </remarks>
  public class IdentityWebFactory : ObjectFactory
  {
    /// <summary>
    /// Fetches MembershipIdentity from the server
    /// </summary>
    /// <param name="criteria"><see cref="MembershipIdentity.Criteria"/></param>
    /// <returns></returns>
    public MembershipIdentity Fetch(MembershipIdentity.Criteria criteria)
    {
      var config = ConfigurationManager.AppSettings["CslaMembershipUseWebServer"];
      if (string.IsNullOrEmpty(config)) config = "True";
      var useWebServer = bool.Parse(config);
      if (useWebServer)
      {
        var appFactory = new IdentityAppFactory();
        return appFactory.Fetch(criteria);
      }
      else
      {
        return Csla.DataPortal.Fetch<MembershipIdentity>(criteria);
      }
    }
  }

  /// <summary>
  /// Verifies user credentials and creates principal/identity based
  /// on the ASP.NET Membership provider.
  /// </summary>
  public class IdentityAppFactory : ObjectFactory
  {
    /// <summary>
    /// Fetches MembershipIdentity from the server
    /// </summary>
    /// <param name="criteria"><see cref="MembershipIdentity.Criteria"/></param>
    /// <returns></returns>
    public MembershipIdentity Fetch(MembershipIdentity.Criteria criteria)
    {
      Type identityType = Type.GetType(criteria.MembershipIdentityType);
      var returnValue = (MembershipIdentity)Activator.CreateInstance(identityType, true);
      LoadIdentity(criteria, returnValue);
      return returnValue;
    }

    /// <summary>
    /// Verifies the user credentials and loads the identity object with
    /// values based on the result.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="identity">Instance of MembershipIdentity or subclass.</param>
    public void LoadIdentity(MembershipIdentity.Criteria criteria, MembershipIdentity identity)
    {
      LoadProperty(identity, MembershipIdentity.AuthenticationTypeProperty, "Membership");
      if (Membership.ValidateUser(criteria.Name, criteria.Password))
      {
        LoadProperty(identity, MembershipIdentity.IsAuthenticatedProperty, true);
        LoadProperty(identity, MembershipIdentity.NameProperty, criteria.Name);
        var roles = new MobileList<string>(Roles.Provider.GetRolesForUser(criteria.Name));
        LoadProperty(identity, MembershipIdentity.RolesProperty, roles);
        identity.LoadCustomData();
      }
      else
      {
        LoadProperty(identity, MembershipIdentity.IsAuthenticatedProperty, false);
        LoadProperty(identity, MembershipIdentity.NameProperty, string.Empty);
        var roles = new MobileList<string>();
        LoadProperty(identity, MembershipIdentity.RolesProperty, roles);
      }
    }
  }
}