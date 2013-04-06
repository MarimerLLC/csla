using System;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Silverlight;
using Csla.Core;

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
#if SILVERLIGHT
    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    public WindowsIdentity() { }
#else
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
    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      _forceInit = 0;
      base.OnDeserialized(context);
    }
#endif

    private void SetWindowsIdentity(MobileList<string> roles, bool isAuthenticated, string name)
    {
      this.LoadProperty(RolesProperty, roles);
      this.LoadProperty(IsAuthenticatedProperty, isAuthenticated);
      this.LoadProperty(NameProperty, name);
    }
    #endregion

    #region Identity and roles population

#if !SILVERLIGHT
    /// <summary>
    /// Retrieves identity and role information from the currently
    /// logged in Windows user.
    /// </summary>
    protected void PopulateWindowsIdentity()
    {
      string DomainDelimiter = "\\";
      System.Security.Principal.IdentityReferenceCollection groups = System.Security.Principal.WindowsIdentity.GetCurrent().Groups;
      MobileList<string> roles = new MobileList<string>();
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
      string identityName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
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

    private static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(new PropertyInfo<MobileList<string>>("Roles"));
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

    private static readonly PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(new PropertyInfo<bool>("IsAuthenticated"));
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

    private static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(new PropertyInfo<string>("Name"));
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
