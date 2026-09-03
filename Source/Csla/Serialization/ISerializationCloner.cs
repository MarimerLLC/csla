//-----------------------------------------------------------------------
// <copyright file="ISerializationCloner.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines an object that can clone objects via serialization</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization
{
  /// <summary>
  /// Defines an object that can clone objects via serialization of
  /// object graphs with improved performance.
  /// </summary>
  public interface ISerializationCloner : ISerializationFormatter
  {
    /// <summary>
    /// Clones an object via serialization into an
    /// object graph.
    /// </summary>
    /// <param name="obj">
    /// Object to be cloned.</param>
    /// <returns>A cloned object graph.</returns>
    object? Clone(object? obj);
  }
}
