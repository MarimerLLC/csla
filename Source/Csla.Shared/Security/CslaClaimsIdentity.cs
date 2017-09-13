#if !NET4
//-----------------------------------------------------------------------
// <copyright file="CslaClaimsIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>ClaimsIdentity subclass that supports serialization by MobileFormatter</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Security.Principal;

namespace Csla.Security
{
  /// <summary>
  /// ClaimsIdentity subclass that supports serialization by MobileFormatter.
  /// </summary>
  [Serializable]
  public class CslaClaimsIdentity : ClaimsIdentity, IMobileObject
  {
    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    public CslaClaimsIdentity()
      :base()
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="authenticationType">Authentication type</param>
    public CslaClaimsIdentity(string authenticationType)
      : base(authenticationType)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="info">Serialization info</param>
    protected CslaClaimsIdentity(System.Runtime.Serialization.SerializationInfo info)
      : base(info)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="identity">Identity object</param>
    public CslaClaimsIdentity(IIdentity identity)
      : base(identity)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="claims">List of claims</param>
    public CslaClaimsIdentity(IEnumerable<Claim> claims)
      : base(claims)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="identity">Identity object</param>
    /// <param name="claims">List of claims</param>
    public CslaClaimsIdentity(IIdentity identity, IEnumerable<Claim> claims)
      : base(identity, claims)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="context">Streaming context</param>
    /// <param name="info">Serialization info</param>
    protected CslaClaimsIdentity(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="authenticationType">Authentication type</param>
    /// <param name="claims">List of claims</param>
    public CslaClaimsIdentity(IEnumerable<Claim> claims, string authenticationType)
      : base(claims, authenticationType)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="authenticationType">Authentication type</param>
    /// <param name="nameType">Name type</param>
    /// <param name="roleType">Role type</param>
    public CslaClaimsIdentity(string authenticationType, string nameType, string roleType)
      : base(authenticationType, nameType, roleType)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="claims">List of claims</param>
    /// <param name="authenticationType">Authentication type</param>
    /// <param name="nameType">Name type</param>
    /// <param name="roleType">Role type</param>
    public CslaClaimsIdentity(IEnumerable<Claim> claims, string authenticationType, string nameType, string roleType)
      : base(claims, authenticationType, nameType, roleType)
    { }

    /// <summary>
    /// Creates a new instance of an object.
    /// </summary>
    /// <param name="identity">Identity object</param>
    /// <param name="claims">List of claims</param>
    /// <param name="authenticationType">Authentication type</param>
    /// <param name="nameType">Name type</param>
    /// <param name="roleType">Role type</param>
    public CslaClaimsIdentity(IIdentity identity, IEnumerable<Claim> claims, string authenticationType, string nameType, string roleType)
      : base(identity, claims, authenticationType, nameType, roleType)
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
    { }

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
    { }

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