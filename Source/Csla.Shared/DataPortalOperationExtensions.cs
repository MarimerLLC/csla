//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object containing information about the</summary>
//-----------------------------------------------------------------------
using System;
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
      switch (operation)
      {
        case DataPortalOperations.Create:
          return AuthorizationActions.CreateObject;
        case DataPortalOperations.Fetch:
          return AuthorizationActions.GetObject;
        case DataPortalOperations.Update:
          return AuthorizationActions.EditObject;
        case DataPortalOperations.Delete:
          return AuthorizationActions.DeleteObject;
        case DataPortalOperations.Execute:
          // CSLA handles Execute/CommandObject as Update operations 
          // - this is the permission that the client DataPortal checks.
          return AuthorizationActions.EditObject;
        default:
          throw new ArgumentOutOfRangeException("operation");
      }
    }

    internal static string ToSecurityActionDescription(this DataPortalOperations operation)
    {
      switch (operation)
      {
        case DataPortalOperations.Create:
          return "create";
        case DataPortalOperations.Fetch:
          return "get";
        case DataPortalOperations.Update:
          return "save";
        case DataPortalOperations.Delete:
          return "delete";
        case DataPortalOperations.Execute:
          return "execute";
        default:
          throw new ArgumentOutOfRangeException("operation");
      }
    }
  }
}
