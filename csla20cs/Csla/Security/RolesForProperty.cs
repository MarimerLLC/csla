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

    /// <summary>
    /// Returns a List(Of String) containing the list
    /// of roles allowed read access.
    /// </summary>
    public List<string> ReadAllowed
    {
      get { return _readAllowed; }
    }

    /// <summary>
    /// Returns a List(Of String) containing the list
    /// of roles denied read access.
    /// </summary>
    public List<string> ReadDenied
    {
      get { return _readDenied; }
    }

    /// <summary>
    /// Returns a List(Of String) containing the list
    /// of roles allowed write access.
    /// </summary>
    public List<string> WriteAllowed
    {
      get { return _writeAllowed; }
    }

    /// <summary>
    /// Returns a List(Of String) containing the list
    /// of roles denied write access.
    /// </summary>
    public List<string> WriteDenied
    {
      get { return _writeDenied; }
    }

    /// <summary>
    /// Returns True if the user is in a role
    /// explicitly allowed read access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns>True if the user is allowed read access.</returns>
    /// <remarks></remarks>
    public bool IsReadAllowed(IPrincipal principal)
    {
      foreach (string role in ReadAllowed)
        if (principal.IsInRole(role))
          return true;
      return false;
    }

    /// <summary>
    /// Returns True if the user is in a role
    /// explicitly denied read access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns>True if the user is denied read access.</returns>
    /// <remarks></remarks>
    public bool IsReadDenied(IPrincipal principal)
    {
      foreach (string role in ReadDenied)
        if (principal.IsInRole(role))
          return true;
      return false;
    }

    /// <summary>
    /// Returns True if the user is in a role
    /// explicitly allowed write access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns>True if the user is allowed write access.</returns>
    /// <remarks></remarks>
    public bool IsWriteAllowed(IPrincipal principal)
    {
      foreach (string role in WriteAllowed)
        if (principal.IsInRole(role))
          return true;
      return false;
    }

    /// <summary>
    /// Returns True if the user is in a role
    /// explicitly denied write access.
    /// </summary>
    /// <param name="principal">A <see cref="System.Security.Principal.IPrincipal" />
    /// representing the user.</param>
    /// <returns>True if the user is denied write access.</returns>
    /// <remarks></remarks>
    public bool IsWriteDenied(IPrincipal principal)
    {
      foreach (string role in WriteDenied)
        if (principal.IsInRole(role))
          return true;
      return false;
    }
  }
}