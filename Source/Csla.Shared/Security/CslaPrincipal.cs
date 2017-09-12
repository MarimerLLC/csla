//-----------------------------------------------------------------------
// <copyright file="CslaPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Base class from which custom principal</summary>
//-----------------------------------------------------------------------
#if NET4
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
#else
using System;
using System.Security.Principal;
using Csla.Serialization.Mobile;
using Csla.Core;
using System.ComponentModel;
using System.Security.Claims;
using System.Collections.Generic;

namespace Csla.Security
{
  /// <summary>
  /// Base class from which custom principal
  /// objects should inherit to operate
  /// properly with the data portal.
  /// </summary>
  [Serializable()]
  public class CslaPrincipal : ClaimsPrincipal, ICslaPrincipal, IMobileObject
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected CslaPrincipal() 
      : base(new UnauthenticatedIdentity())
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identity">Identity object for the user.</param>
    protected CslaPrincipal(IIdentity identity)
      : base(identity)
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identity">Identity object for the user.</param>
    protected CslaPrincipal(IEnumerable<CslaIdentity> identity)
      : base(identity)
    { }

    #region IMobileObject serialization code

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      OnGetState(info, StateMode.Serialization);
    }

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("CslaPrincipal.Identity", MobileFormatter.Serialize(_identity));
    }

    /// <summary>
    /// Override this method to insert your child object
    /// references into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    { }

    void IMobileObject.SetState(SerializationInfo info)
    {
      OnSetState(info, StateMode.Serialization);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnSetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetState(SerializationInfo info, StateMode mode)
    {
      _identity = (IIdentity)MobileFormatter.Deserialize(info.GetValue<byte[]>("CslaPrincipal.Identity"));
    }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    { }

#endregion
  }
}
#endif