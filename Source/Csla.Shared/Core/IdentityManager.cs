//-----------------------------------------------------------------------
// <copyright file="IdentityManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Used by the root object in a graph to manage</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Used by the root object in a graph to manage
  /// the object instance identity values for the
  /// graph.
  /// </summary>
  public class IdentityManager
  {
    private int _lastIdentity;

    /// <summary>
    /// Gets and consumes the next available unique identity value 
    /// for an object instance in the object graph. Implemented by
    /// the root object of the graph.
    /// </summary>
    /// <returns>The next available identity value.</returns>
    public int GetNextIdentity()
    {
      return _lastIdentity++;
    }
  }
}