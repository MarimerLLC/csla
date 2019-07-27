//-----------------------------------------------------------------------
// <copyright file="IdentityManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
    private int _nextIdentity;

    /// <summary>
    /// Gets and consumes the next available unique identity value 
    /// for an object instance in the object graph. Implemented by
    /// the root object of the graph.
    /// </summary>
    /// <param name="current">Current identity value for object.</param>
    /// <returns>The next available identity value.</returns>
    public int GetNextIdentity(int current)
    {
      if (current >= _nextIdentity)
      {
        _nextIdentity = current + 1;
        return current;
      }
      else
      {
        return _nextIdentity++;
      }
    }
  }
}