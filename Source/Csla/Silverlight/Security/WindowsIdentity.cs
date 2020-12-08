//-----------------------------------------------------------------------
// <copyright file="WindowsIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class to simplify the retrieval of Windows identity</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Core;
using System.Runtime.Serialization;

namespace Csla.Silverlight.Security
{
  //[MobileFactory("Csla.Security.IdentityFactory,Csla", "FetchWindowsIdentity")]
  /// <summary>
  /// Base class to simplify the retrieval of Windows identity
  /// information from a Windows server to a 
  /// CSLA .NET for Silverlight client.
  /// </summary>
  [Serializable()]
  public abstract class WindowsIdentity : ReadOnlyBase<WindowsIdentity>,
      System.Security.Principal.IIdentity, Csla.Security.ICheckRoles
  {
    #region Constructor, Helper Setter
    private static int _forceInit;

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    public WindowsIdentity()
    {
      _forceInit = _forceInit + 0;
    }

    /// <summary>
    /// Method invoked when the object is deserialized.
    /// </summary>
    /// <param name="context">Serialization context.</param>
    protected override void OnDeserialized(StreamingContext context)
    {
      _forceInit = _forceInit + 0;
      base.OnDeserialized(context);
    }

    private void SetWindowsIdentity(MobileList<string> roles, bool isAuthenticated, string name)
    {
      this.LoadProperty(RolesProperty, roles);
      this.LoadProperty(IsAuthenticatedProperty, isAuthenticated);
      this.LoadProperty(NameProperty, name);
    }
    #endregion

    #region Identity and roles population

#if NETSTANDARD2_0 || NET5_0
    /// <summary>
    /// Retrieves identity and role information from the currently
    /// logged in Windows user.
    /// </summary>
    protected virtual void PopulateWindowsIdentity()
    {
      throw new NotImplementedException();
    }
#endif
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0 && !NET5_0
    /// <summary>
    /// Retrieves identity and role information from the currently
    /// logged in Windows user.
    /// </summary>
    protected void PopulateWindowsIdentity()
    {
      string DomainDelimiter = "\\";
      MobileList<string> roles = new MobileList<string>();
      string identityName = string.Empty;

      var currentUser = System.Security.Principal.WindowsIdentity.GetCurrent();
      
#if !MONO
      System.Security.Principal.IdentityReferenceCollection groups = System.Security.Principal.WindowsIdentity.GetCurrent().Groups;
      foreach (System.Security.Principal.IdentityReference item in groups)
      {
        System.Security.Principal.NTAccount account = (System.Security.Principal.NTAccount)item.Translate(typeof(System.Security.Principal.NTAccount));
        if (account.Value.Contains(DomainDelimiter))
        {
          roles.Add(account.Value.Substring(account.Value.LastIndexOf(DomainDelimiter) + 1));
        }
        else
        {
          roles.Add(account.Value);
        }
      }
#endif
      
      if (currentUser != null) 
        identityName = currentUser.Name;
      if (identityName.Contains(DomainDelimiter))
      {
        identityName = identityName.Substring(identityName.LastIndexOf(DomainDelimiter) + 1);
      }
      this.LoadProperty(RolesProperty, roles);
      this.LoadProperty(IsAuthenticatedProperty, true);
      this.LoadProperty(NameProperty, identityName);
    }
#endif

    #endregion

    #region  IsInRole

    /// <summary>
    /// Property info for Roles property
    /// </summary>
    public static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(new PropertyInfo<MobileList<string>>("Roles"));

    /// <summary>
    /// Gets or sets the list of roles for this user.
    /// </summary>
    protected MobileList<string> Roles
    {
      get { return ReadProperty(RolesProperty); }
      set { LoadProperty(RolesProperty, value); }
    }

    /// <summary>
    /// Returns a value indicating whether the current user
    /// is in the specified role.
    /// </summary>
    /// <param name="role">Role to check.</param>
    /// <returns></returns>
    public bool IsInRole(string role)
    {
      return GetProperty<MobileList<string>>(RolesProperty).Contains(role);
    }

    #endregion

    #region  IIdentity

    /// <summary>
    /// Returns the authentication type for this identity.
    /// Always returns Windows.
    /// </summary>
    public string AuthenticationType
    {
      get
      {
        return "Windows";
      }
    }

    /// <summary>
    /// Property info for IsAuthenticated property
    /// </summary>
    public static readonly PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(new PropertyInfo<bool>("IsAuthenticated"));

    /// <summary>
    /// Returns a value indicating whether this identity
    /// represents an authenticated user.
    /// </summary>
    public bool IsAuthenticated
    {
      get
      {
        return GetProperty<bool>(IsAuthenticatedProperty);
      }
    }

    /// <summary>
    /// Property info for Name property
    /// </summary>
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(new PropertyInfo<string>("Name"));

    /// <summary>
    /// Returns the name of the user.
    /// </summary>
    public string Name
    {
      get
      {
        return GetProperty<string>(NameProperty);
      }
    }

    #endregion
  }

}