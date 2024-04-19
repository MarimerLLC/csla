//-----------------------------------------------------------------------
// <copyright file="IContainsDeletedList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface implemented by a list which tracks its deleted items</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla
{
  /// <summary>
  /// Defines an object that holds a list of deleted items.
  /// </summary>
  public interface IContainsDeletedList
  {
    /// <summary>
    /// List of deleted child objects
    /// </summary>
    IEnumerable<IEditableBusinessObject> DeletedList { get; }
  }
}
