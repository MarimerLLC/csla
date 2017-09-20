#if !NET40 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !PCL46
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
using System.Linq;

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
    public CslaClaimsPrincipal()
      : base(new ClaimsIdentity())
    { }

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

#if !NET45
    /// <summary>
    /// Creates an instance of the object from
    /// a binary stream.
    /// </summary>
    /// <param name="reader">Binary reader</param>
    public CslaClaimsPrincipal(System.IO.BinaryReader reader)
      : base(reader)
    { }
#endif
#if !WINDOWS_UWP
    /// <summary>
    /// Creates an instance of the object from
    /// BinaryFormatter or NDCS deserialization.
    /// </summary>
    /// <param name="context">Serialization context</param>
    /// <param name="info">Serialization info</param>
    protected CslaClaimsPrincipal(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    { }
#endif

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
      int index = 0;
      info.AddValue($"i.Count", Identities.Count());
      foreach (var item in Identities)
      {
        // TODO: Actor is a ClaimsIdentity
        //info.AddValue($"i{index}.Actor", item.Actor);
        info.AddValue($"i{index}.AuthenticationType", item.AuthenticationType);
        info.AddValue($"i{index}.IsAuthenticated", item.IsAuthenticated);
        info.AddValue($"i{index}.Label", item.Label);
        info.AddValue($"i{index}.Name", item.Name);
        info.AddValue($"i{index}.RoleClaimType", item.RoleClaimType);
        info.AddValue($"i{index}.Claims", item.Claims.Count());
        int claimIndex = 0;
        foreach (var claim in item.Claims)
        {
          info.AddValue($"i{index}.c{claimIndex}.Issuer", claim.Issuer);
          info.AddValue($"i{index}.c{claimIndex}.OriginalIssuer", claim.OriginalIssuer);
          // TODO: Subject is a ClaimsIdentity
          //info.AddValue($"i{index}.c{claimIndex}.Subject", claim.Subject);
          info.AddValue($"i{index}.c{claimIndex}.Type", claim.Type);
          info.AddValue($"i{index}.c{claimIndex}.Value", claim.Value);
          info.AddValue($"i{index}.c{claimIndex}.ValueType", claim.ValueType);
          // TODO: serialize claim.Properties dictionary
          claimIndex++;
        }
        index++;
      }
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
      var count = info.GetValue<int>("i.Count");
      for (int index = 0; index < count; index++)
      {
        var claimCount = info.GetValue<int>($"i{index}.Claims");
        var claims = new List<Claim>();
        for (int claimIndex = 0; claimIndex < claimCount; claimIndex++)
        {
          var issuer = info.GetValue<string>($"i{index}.c{claimIndex}.Issuer");
          var originalIssuer = info.GetValue<string>($"i{index}.c{claimIndex}.OriginalIssuer");
          // TODO: deserialize subject identity object
          //var subject = info.GetValue<string>($"i{index}.c{claimIndex}.Subject");
          var type = info.GetValue<string>($"i{index}.c{claimIndex}.Type");
          var value = info.GetValue<string>($"i{index}.c{claimIndex}.Value");
          var valueType = info.GetValue<string>($"i{index}.c{claimIndex}.ValueType");
          var claim = new Claim(type, value, valueType, issuer, originalIssuer);
          claims.Add(claim);
        }
        // TODO: deserialize actor identity object
        //var actor = info.GetValue<string>($"i{index}.Actor");
        var authenticationType = info.GetValue<string>($"i{index}.AuthenticationType");
        var isAuthenticated = info.GetValue<string>($"i{index}.IsAuthenticated");
        var label = info.GetValue<string>($"i{index}.Label");
        var name = info.GetValue<string>($"i{index}.Name");
        var roleClaimType = info.GetValue<string>($"i{index}.RoleClaimType");
        var identity = new ClaimsIdentity(claims, authenticationType, name, roleClaimType);
        identity.Label = label;
        // TODO: deserialize Properties dictionary
      }
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