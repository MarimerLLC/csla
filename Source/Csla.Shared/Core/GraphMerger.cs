//-----------------------------------------------------------------------
// <copyright file="GrapherMerger.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines members required for smart</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Implements behavior to merge one object graph
  /// into a clone of itself (typically post-serialization).
  /// </summary>
  public class GraphMerger : Csla.Server.ObjectFactory
  {
    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    public void MergeGraph(IBusinessObject target, IBusinessObject source)
    {
      if (target is IBusinessBase)
        MergeInstance((IBusinessBase)target, (IBusinessBase)source);
    }

    private void MergeInstance(IBusinessBase target, IBusinessBase source)
    {
      var imp = target as IManageProperties;
      if (imp != null)
      {
        var targetProperties = imp.GetManagedProperties();
        foreach (var item in targetProperties)
          LoadProperty(target, item, ReadProperty(source, item));
        if (source.IsNew)
          MarkNew(target);
        else if (!source.IsDirty)
          MarkOld(target);
        CheckRules(target);
      }
    }
  }
}
