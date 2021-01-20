//-----------------------------------------------------------------------
// <copyright file="CslaClaimsPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>ClaimsPrincipal subclass that supports serialization by MobileFormatter</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using Csla.Serialization.Mobile;
using System.Security.Claims;
using System.Collections.Generic;

namespace Csla.Security
{
  /// <summary>
  /// ClaimsPrincipal subclass that supports serialization by SerializationFormatterFactory.GetFormatter().
  /// </summary>
  [Serializable()]
  public class CslaClaimsPrincipal : ClaimsPrincipal, ICslaPrincipal
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public CslaClaimsPrincipal()
      : base(new ClaimsIdentity())
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="principal">Source principal from which to copy identity</param>
    public CslaClaimsPrincipal(ClaimsPrincipal principal)
      : base(principal.Identities)
    { }

    /// <summary>
    /// Creates an instance of the object, initializing a role
    /// claim for each role in the identity's Roles collection.
    /// </summary>
    /// <param name="identity">Identity object for the user.</param>
    public CslaClaimsPrincipal(ICslaIdentity identity)
      : base(identity)
    {
      var baseidentity = (ClaimsIdentity)base.Identity;
      if (identity.Roles != null)
        foreach (var item in identity.Roles)
          baseidentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", item));
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identity">Identity object for the user.</param>
    public CslaClaimsPrincipal(IIdentity identity)
      : base(identity)
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="identities">List of claims identity objects for the user.</param>
    public CslaClaimsPrincipal(IEnumerable<ClaimsIdentity> identities)
      : base(identities)
    { }

    /// <summary>
    /// Creates an instance of the object based on an
    /// existing principal object.
    /// </summary>
    /// <param name="principal">Principal object</param>
    public CslaClaimsPrincipal(IPrincipal principal)
      : base(principal)
    { }

    /// <summary>
    /// Creates an instance of the object from
    /// a binary stream.
    /// </summary>
    /// <param name="reader">Binary reader</param>
    public CslaClaimsPrincipal(System.IO.BinaryReader reader)
      : base(reader)
    { }

    /// <summary>
    /// Creates an instance of the object from
    /// BinaryFormatter or NDCS deserialization.
    /// </summary>
    /// <param name="context">Serialization context</param>
    /// <param name="info">Serialization info</param>
    protected CslaClaimsPrincipal(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    { }

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    { }

    void IMobileObject.GetState(SerializationInfo info)
    { }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    { }

    void IMobileObject.SetState(SerializationInfo info)
    { }
  }
}
