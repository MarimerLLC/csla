//-----------------------------------------------------------------------
// <copyright file="AuthorizeRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object containing information about the</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Server
{
  /// <summary>
  /// Object containing information about the
  /// client request to the data portal.
  /// </summary>
  public class AuthorizeRequest
  {
    /// <summary>
    /// Gets the type of business object affected by
    /// the client request.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type ObjectType { get; }
    /// <summary>
    /// Gets a reference to the criteria or 
    /// business object passed from
    /// the client to the server.
    /// </summary>
    public object? RequestObject { get; }
    /// <summary>
    /// Gets the data portal operation requested
    /// by the client.
    /// </summary>
    public DataPortalOperations Operation { get; }

    internal AuthorizeRequest([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object? requestObject, DataPortalOperations operation)
    {
      ObjectType = objectType;
      RequestObject = requestObject;
      Operation = operation;
    }
  }
}