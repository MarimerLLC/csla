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
  public class FromServicesAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to initialize a new
  /// domain object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class CreateAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to load existing data into
  /// the domain object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class FetchAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to insert domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class InsertAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to update domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class UpdateAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete domain object data
  /// during an explicit delete operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteSelfAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to initialize a new
  /// child object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class CreateChildAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to load existing data into
  /// the child object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class FetchChildAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to insert child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class InsertChildAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to update child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class UpdateChildAttribute : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteSelfChildAttribute : Attribute
  { }
}
