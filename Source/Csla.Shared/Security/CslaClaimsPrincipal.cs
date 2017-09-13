#if !NET4
//-----------------------------------------------------------------------
// <copyright file="CslaClaimsPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>ClaimsPrincipal subclass that supports serialization by MobileFormatter</summary>
//-----------------------------------------------------------------------
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
  /// ClaimsPrincipal subclass that supports serialization by MobileFormatter.
  /// </summary>
  [Serializable()]
  public class CslaClaimsPrincipal : ClaimsPrincipal, ICslaPrincipal
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected CslaClaimsPrincipal()
      : base(new UnauthenticatedIdentity())
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identity">Identity object for the user.</param>
    protected CslaClaimsPrincipal(IIdentity identity)
      : base(identity)
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identity">List of claims identity objects for the user.</param>
    protected CslaClaimsPrincipal(IEnumerable<CslaClaimsIdentity> identity)
      : base(identity)
    { }

    /// <summary>
    /// Creates an instance of the object based on an
    /// existing principal object.
    /// </summary>
    /// <param name="principal">Principal object</param>
    protected CslaClaimsPrincipal(IPrincipal principal)
      : base(principal)
    { }

    /// <summary>
    /// Creates an instance of the object from
    /// BinaryFormatter or NDCS deserialization.
    /// </summary>
    /// <param name="context">Serialization context</param>
    /// <param name="info">Serialization info</param>
    public CslaClaimsPrincipal(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
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