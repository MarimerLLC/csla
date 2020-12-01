//-----------------------------------------------------------------------
// <copyright file="AuthorizeRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object containing information about the</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Properties;
using Csla.Rules;
using Csla.Security;

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
    public Type ObjectType { get; private set; }
    /// <summary>
    /// Gets a reference to the criteria or 
    /// business object passed from
    /// the client to the server.
    /// </summary>
    public object RequestObject { get; private set; }
    /// <summary>
    /// Gets the data portal operation requested
    /// by the client.
    /// </summary>
    public DataPortalOperations Operation { get; private set; }

    internal AuthorizeRequest(Type objectType, object requestObject, DataPortalOperations operation)
    {
      this.ObjectType = objectType;
      this.RequestObject = requestObject;
      this.Operation = operation;
    }

    /// <summary>
    /// Checks that the current identity has permission to carry out this operation,
    /// and if not throws a <c>SecurityException</c>.
    /// </summary>
    /// <exception cref="SecurityException">Thrown if the current principal does 
    /// not have permission to carry out this operation.</exception>
    public void CheckPermissions()
    {
      if (Operation == DataPortalOperations.Update || 
          Operation == DataPortalOperations.Execute)
      { 
         // Per-Instance checks
         if (!BusinessRules.HasPermission(Operation.ToAuthAction(), RequestObject))
         {
            throw new SecurityException(
               string.Format(Resources.UserNotAuthorizedException,
                   Operation.ToSecurityActionDescription(),
                   ObjectType.Name)
               );
         }
      }

      // Per-Type checks
      if (!BusinessRules.HasPermission(Operation.ToAuthAction(), ObjectType))
      {
         throw new SecurityException(
            string.Format(Resources.UserNotAuthorizedException,
                Operation.ToSecurityActionDescription(),
                ObjectType.Name)
            );
      }
    }
  }
}