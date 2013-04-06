using System;
using System.Collections.Generic;

namespace Csla.Security
{
  internal class RolesForType
  {
    private List<string> _allowCreateRoles;
    internal List<string> AllowCreateRoles
    {
      get { return _allowCreateRoles; }
    }

    private List<string> _denyCreateRoles;
    internal List<string> DenyCreateRoles
    {
      get { return _denyCreateRoles; }
    }

    private List<string> _allowGetRoles;
    internal List<string> AllowGetRoles
    {
      get { return _allowGetRoles; }
    }

    private List<string> _denyGetRoles;
    internal List<string> DenyGetRoles
    {
      get { return _denyGetRoles; }
    }

    private List<string> _allowEditRoles;
    internal List<string> AllowEditRoles
    {
      get { return _allowEditRoles; }
    }

    private List<string> _denyEditRoles;
    internal List<string> DenyEditRoles
    {
      get { return _denyEditRoles; }
    }

    private List<string> _allowDeleteRoles;
    internal List<string> AllowDeleteRoles
    {
      get { return _allowDeleteRoles; }
    }

    private List<string> _denyDeleteRoles;
    internal List<string> DenyDeleteRoles
    {
      get { return _denyDeleteRoles; }
    }

    /// <summary>
    /// Specify the roles allowed to get (fetch)
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void AllowGet(params string[] roles)
    {
      if (_allowGetRoles == null)
        _allowGetRoles = new List<string>();
      _allowGetRoles.AddRange(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to get (fetch)
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void DenyGet(params string[] roles)
    {
      if (_denyGetRoles == null)
        _denyGetRoles = new List<string>();
      _denyGetRoles.AddRange(roles);
    }

    /// <summary>
    /// Specify the roles allowed to edit (save)
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void AllowEdit(params string[] roles)
    {
      if (_allowEditRoles == null)
        _allowEditRoles = new List<string>();
      _allowEditRoles.AddRange(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to edit (save)
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void DenyEdit(params string[] roles)
    {
      if (_denyEditRoles == null)
        _denyEditRoles = new List<string>();
      _denyEditRoles.AddRange(roles);
    }

    /// <summary>
    /// Specify the roles allowed to create
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void AllowCreate(params string[] roles)
    {
      if (_allowCreateRoles == null)
        _allowCreateRoles = new List<string>();
      _allowCreateRoles.AddRange(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to create
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void DenyCreate(params string[] roles)
    {
      if (_denyCreateRoles == null)
        _denyCreateRoles = new List<string>();
      _denyCreateRoles.AddRange(roles);
    }

    /// <summary>
    /// Specify the roles allowed to delete
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void AllowDelete(params string[] roles)
    {
      if (_allowDeleteRoles == null)
        _allowDeleteRoles = new List<string>();
      _allowDeleteRoles.AddRange(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to delete
    /// a given type of business object.
    /// </summary>
    /// <param name="roles">List of roles.</param>
    internal void DenyDelete(params string[] roles)
    {
      if (_denyDeleteRoles == null)
        _denyDeleteRoles = new List<string>();
      _denyDeleteRoles.AddRange(roles);
    }
  }
}
