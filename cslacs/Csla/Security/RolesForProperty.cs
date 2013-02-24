using System;
using System.Collections.Generic;
using System.Security.Principal;
using Csla.Serialization;

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
    private List<string> _readAllowed;
    private List<string> _readDenied;
    private List<string> _writeAllowed;
    private List<string> _writeDenied;
    private List<string> _executeAllowed;
    private List<string> _executeDenied;

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles allowed read access.
    /// </summary>
    public List<string> ReadAllowed
    {
      get
      {
        if (_readAllowed == null)
        {
          _readAllowed = new List<string>();
        }

        return _readAllowed;
      }
    }

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles denied read access.
    /// </summary>
    public List<string> ReadDenied
    {
      get
      {
        if (_readDenied == null)
        {
          _readDenied = new List<string>();
        }

        return _readDenied;
      }
    }

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles allowed write access.
    /// </summary>
    public List<string> WriteAllowed
    {
      get
      {
        if (_writeAllowed == null)
        {
          _writeAllowed = new List<string>();
        }

        return _writeAllowed;
      }
    }

    /// <summary>
    /// Returns a List(Of string) containing the list
    /// of roles denied write access.
    /// </summary>
    public List<string> WriteDenied
    {
      get
      {
        if (_writeDenied == null)
        {
          _writeDenied = new List<string>();
        }

        return _writeDenied;
      }
    }

    /// <summary>
    /// Returns a List(Of String) containing the list
    /// of roles allowed execute access.
    /// </summary>
    public List<string> ExecuteAllowed
    {
      get
      {
        if (_executeAllowed == null)
        {
          _executeAllowed = new List<string>();
        }

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
        if (_executeDenied == null)
        {
          _executeDenied = new List<string>();
        }

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
      return AuthorizationRulesManager.PrincipalRoleInList(principal, ReadAllowed);
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
      return AuthorizationRulesManager.PrincipalRoleInList(principal, ReadDenied);
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
      return AuthorizationRulesManager.PrincipalRoleInList(principal, WriteAllowed);
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
      return AuthorizationRulesManager.PrincipalRoleInList(principal, WriteDenied);
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
      return AuthorizationRulesManager.PrincipalRoleInList(principal, ExecuteAllowed);
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
      return AuthorizationRulesManager.PrincipalRoleInList(principal, ExecuteDenied);
    }
  }
}