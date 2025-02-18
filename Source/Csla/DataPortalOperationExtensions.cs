//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object containing information about the</summary>
//-----------------------------------------------------------------------

using Csla.Rules;

namespace Csla
{
  /// <summary>
  /// Extension methods for mapping <c>DataPortalOperations</c> to other system values.
  /// </summary>
  internal static class DataPortalOperationExtensions
  {
    internal static AuthorizationActions ToAuthAction(this DataPortalOperations operation)
    {
      return operation switch
      {
        DataPortalOperations.Create => AuthorizationActions.CreateObject,
        DataPortalOperations.Fetch => AuthorizationActions.GetObject,
        DataPortalOperations.Update => AuthorizationActions.EditObject,
        DataPortalOperations.Delete => AuthorizationActions.DeleteObject,
        DataPortalOperations.Execute =>
          // CSLA handles Execute/CommandObject as Update operations 
          // - this is the permission that the client DataPortal checks.
          AuthorizationActions.EditObject,
        _ => throw new ArgumentOutOfRangeException(nameof(operation))
      };
    }

    internal static string ToSecurityActionDescription(this DataPortalOperations operation)
    {
      return operation switch
      {
        DataPortalOperations.Create => "create",
        DataPortalOperations.Fetch => "get",
        DataPortalOperations.Update => "save",
        DataPortalOperations.Delete => "delete",
        DataPortalOperations.Execute => "execute",
        _ => throw new ArgumentOutOfRangeException(nameof(operation))
      };
    }
  }
}
