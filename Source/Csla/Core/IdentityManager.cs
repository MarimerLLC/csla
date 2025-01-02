﻿//-----------------------------------------------------------------------
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
    public int GetNextIdentity(int? current = null)
    {
      int result;
      if (current == null) 
      { 
        result = _nextIdentity;
      }
      else if (current == -1)
      {
        result = _nextIdentity++;
      }
      else
      {
        result = current.Value;
        if (current >= _nextIdentity)
        {
          _nextIdentity = current.Value + 1;
        }
      }
      return result;
    }

    /// <summary>
    /// Ensures that the internal value of <see cref="_nextIdentity"/> is greater than the greatest <see cref="IBusinessObject.Identity"/> within the given collection.
    /// That ensures that new object get a unique identity within the collection.
    /// </summary>
    /// <typeparam name="T">Item type of the list</typeparam>
    /// <param name="parent"></param>
    /// <param name="items"></param>
    internal static void EnsureNextIdentityValueIsUnique<T>(IParent parent, IReadOnlyCollection<T> items) where T: IBusinessObject
    {
      // No items means we do not have to worry about any identity duplicates
      if (items.Count == 0)
      {
        return;
      }

      _ = parent.GetNextIdentity(items.Max(c => c.Identity));
    }
  }
}