//-----------------------------------------------------------------------
// <copyright file="CslaPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class from which custom principal</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using Csla.Serialization.Mobile;

namespace Csla.Security
{
  /// <summary>
  /// Base class from which custom principal
  /// objects should inherit to operate
  /// properly with the data portal.
  /// </summary>
  [Serializable()]
  public class CslaPrincipal : Csla.Core.MobileObject, IPrincipal, ICslaPrincipal
  {
    private IIdentity _identity;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected CslaPrincipal() { _identity = new UnauthenticatedIdentity(); }

    /// <summary>
    /// Returns the user's identity object.
    /// </summary>
    public virtual IIdentity Identity
    {
      get { return _identity; }
    }

    /// <summary>
    /// Returns a value indicating whether the
    /// user is in a given role.
    /// </summary>
    /// <param name="role">Name of the role.</param>
    public virtual bool IsInRole(string role)
    {
      var check = _identity as ICheckRoles;
      if (check != null)
        return check.IsInRole(role);
      else
        return false;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identity">Identity object for the user.</param>
    protected CslaPrincipal(IIdentity identity)
    {
      _identity = identity;
    }

    /// <summary>
    /// Override this method to get custom field values
    /// from the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="mode">Serialization mode.</param>
    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
    {
      info.AddValue("CslaPrincipal.Identity", MobileFormatter.Serialize(_identity));
      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Override this method to set custom field values
    /// ito the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="mode">Serialization mode.</param>
    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
    {
      base.OnSetState(info, mode);
      _identity = (IIdentity)MobileFormatter.Deserialize(info.GetValue<byte[]>("CslaPrincipal.Identity"));
    }
  }
}