//-----------------------------------------------------------------------
// <copyright file="IGraphMergeable.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Lets a collection call the strongly typed GraphMerger method for itself</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Implemented by editable collections so <see cref="GraphMerger"/> can
  /// call the merge method matching the collection's generic type arguments
  /// without creating generic method instantiations at runtime.
  /// </summary>
  internal interface IGraphMergeable
  {
    void MergeGraph(GraphMerger merger, IEditableCollection source);
    Task MergeGraphAsync(GraphMerger merger, IEditableCollection source);
  }
}
