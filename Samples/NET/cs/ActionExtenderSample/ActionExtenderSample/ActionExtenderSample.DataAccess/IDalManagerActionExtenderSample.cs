//-----------------------------------------------------------------------
// <copyright file="IDalManagerActionExtenderSample.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------
using System;

namespace ActionExtenderSample.DataAccess
{
  /// <summary>
  /// Defines the ActionExtenderSample DAL manager interface for DAL providers.
  /// </summary>
  public interface IDalManagerActionExtenderSample : IDisposable
  {
    /// <summary>
    /// Gets the DAL provider for a given object Type.
    /// </summary>
    /// <typeparam name="T">Object Type that requires a ActionExtenderSample DAL provider.</typeparam>
    /// <returns>A new ActionExtenderSample DAL instance for the given Type.</returns>
    T GetProvider<T>() where T : class;
  }
}
