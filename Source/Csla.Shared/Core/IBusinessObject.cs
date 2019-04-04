//-----------------------------------------------------------------------
// <copyright file="IBusinessObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the core interface implemented</summary>
//-----------------------------------------------------------------------
namespace Csla.Core
{
  /// <summary>
  /// This is the core interface implemented
  /// by all CSLA .NET base classes.
  /// </summary>
  public interface IBusinessObject
  {
    /// <summary>
    /// Gets a value representing this object instance's
    /// unique identity value within the business object
    /// graph.
    /// </summary>
    int Identity { get; }
  }
}