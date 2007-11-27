using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.ComponentModel;

namespace Csla.Security
{

  /// <summary>
  /// Maintains a list of allowed and denied
  /// user roles for a specific property.
  /// </summary>
  /// <remarks></remarks>
  [Serializable()]
  internal class RolesForProperty
  {
    private List<string> _readAllowed = new List<string>();
    private List<string> _readDenied = new List<string>();
    private List<string> _writeAllowed = new List<string>();
    private List<string> _writeDenied = new List<string>();
    private List<string> _executeAllowed = new List<string>();
    private List<string> _executeDenied = new List<string>();

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles allowed read access.
    /// </summary>
    public List<string> ReadAllowed
    {
      get { return _readAllowed; }
    }

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles denied read access.
    /// </summary>
    public List<string> ReadDenied
    {
      get { return _readDenied; }
    }

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles allowed write access.
    /// </summary>
    public List<string> WriteAllowed
    {
      get { return _writeAllowed; }
    }

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles denied write access.
    /// </summary>
    public List<string> WriteDenied
    {
      get { return _writeDenied; }
    }

    /// <summary>
    /// Returns a List(Of String) containing the list
    /// of roles allowed execute access.
    /// </summary>
    public List<string> ExecuteAllowed
    {
      get
      {
        return _executeAllowed;
      }
    }

    /// <summary>
    /// Returns a List(Of String) containing the list
    /// of roles denied execute access.
    /// </summary>
    public List<string> ExecuteDenied
    {
      get
      {
        return _executeDenied;
      }
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is in a role
    /// explicitly allowed read access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns><see langword="true" /> if the user is allowed read access.</returns>
    /// <remarks></remarks>
    public bool IsReadAllowed(IPrincipal principal)
    {
      foreach (string role in ReadAllowed)
        if (IsInRole(principal, role))
          return true;
      return false;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is in a role
    /// explicitly denied read access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns><see langword="true" /> if the user is denied read access.</returns>
    /// <remarks></remarks>
    public bool IsReadDenied(IPrincipal principal)
    {
      foreach (string role in ReadDenied)
        if (IsInRole(principal, role))
          return true;
      return false;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is in a role
    /// explicitly allowed write access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns><see langword="true" /> if the user is allowed write access.</returns>
    /// <remarks></remarks>
    public bool IsWriteAllowed(IPrincipal principal)
    {
      foreach (string role in WriteAllowed)
        if (IsInRole(principal, role))
          return true;
      return false;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the user is in a role
    /// explicitly denied write access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns><see langword="true" /> if the user is denied write access.</returns>
    /// <remarks></remarks>
    public bool IsWriteDenied(IPrincipal principal)
    {
      foreach (string role in WriteDenied)
        if (IsInRole(principal, role))
          return true;
      return false;
    }

    /// <summary>
    /// Returns True if the user is in a role
    /// explicitly allowed execute access.
    /// </summary>
    /// <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    /// <returns>True if the user is allowed execute access.</returns>
    /// <remarks></remarks>
    public bool IsExecuteAllowed(IPrincipal principal)
    {

      bool result = false;
      foreach (string role in ExecuteAllowed)
      {
        if (IsInRole(principal, role))
        {
          result = true;
          break;
        }
      }
      return result;

    }

    /// <summary>
    /// Returns True if the user is in a role
    /// explicitly denied execute access.
    /// </summary>
    /// <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    /// <returns>True if the user is denied execute access.</returns>
    /// <remarks></remarks>
    public bool IsExecuteDenied(IPrincipal principal)
    {

      bool result = false;
      foreach (string role in ExecuteDenied)
      {
        if (IsInRole(principal, role))
        {
          result = true;
          break;
        }
      }
      return result;

    }

    private static IsInRoleProvider mIsInRoleProvider;

    private bool IsInRole(IPrincipal principal, string role)
    {
      if (mIsInRoleProvider == null)
      {
        string provider = ApplicationContext.IsInRoleProvider;
        if (string.IsNullOrEmpty(provider))
          mIsInRoleProvider = IsInRoleDefault;
        else
        {
          string[] items = provider.Split(',');
          Type containingType = Type.GetType(items[0] + "," + items[1]);
          mIsInRoleProvider = (IsInRoleProvider)(Delegate.CreateDelegate(typeof(IsInRoleProvider), containingType, items[2]));
        }
      }
      return mIsInRoleProvider(principal, role);
    }

    private bool IsInRoleDefault(IPrincipal principal, string role)
    {
      return IsInRole(principal, role);
    }
  }

  /// <summary>
  /// Delegate for the method called when the a role
  /// needs to be checked for the current user.
  /// </summary>
  /// <param name="principal">
  /// The current security principal object.
  /// </param>
  /// <param name="role">
  /// The role to be checked.
  /// </param>
  /// <returns>
  /// True if the current user is in the specified role.
  /// </returns>
  public delegate bool IsInRoleProvider(IPrincipal principal, string role);
}