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
  public class FromServices : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to initialize a new
  /// domain object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class Create : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to load existing data into
  /// the domain object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class Fetch : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to insert domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class Insert : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to update domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class Update : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete domain object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class Delete : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete domain object data
  /// during an explicit delete operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteSelf : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to initialize a new
  /// child object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class CreateChild : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to load existing data into
  /// the child object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class FetchChild : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to insert child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class InsertChild : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to update child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class UpdateChild : Attribute
  { }

  /// <summary>
  /// Specifies a method used by the server-side
  /// data portal to delete child object data
  /// during an update operation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DeleteChild : Attribute
  { }
}
