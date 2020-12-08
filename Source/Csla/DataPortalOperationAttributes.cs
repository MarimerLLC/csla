//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationAttributes.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the attributes used by data portal to find methods</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// Specifies a parameter that is provided
  /// via dependency injection.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public class InjectAttribute : Attribute
  { }

  /// <summary>
  /// Base type for data portal operation
  /// attributes.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DataPortalOperationAttribute : Attribute
  { }

  /// <summary>
  /// Base type for data portal root operation
  /// attributes.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DataPortalRootOperationAttribute : DataPortalOperationAttribute
  { }

  /// <summary>
  /// Base type for data portal child operation
  /// attributes.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DataPortalChildOperationAttribute : DataPortalOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to initialize a new
  /// domain object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class CreateAttribute : DataPortalRootOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to load existing data into
  /// the domain object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class FetchAttribute : DataPortalRootOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to insert domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class InsertAttribute : DataPortalRootOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to update domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class UpdateAttribute : DataPortalRootOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to execute a command
  /// object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class ExecuteAttribute : DataPortalRootOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteAttribute : DataPortalRootOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete domain object data
  /// during an explicit delete operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteSelfAttribute : DataPortalRootOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to initialize a new
  /// child object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class CreateChildAttribute : DataPortalChildOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to load existing data into
  /// the child object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class FetchChildAttribute : DataPortalChildOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to insert child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class InsertChildAttribute : DataPortalChildOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to update child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class UpdateChildAttribute : DataPortalChildOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteSelfChildAttribute : DataPortalChildOperationAttribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to execute a child command
  /// object during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class ExecuteChildAttribute : DataPortalChildOperationAttribute
  { }
}
