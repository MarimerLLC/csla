//-----------------------------------------------------------------------
// <copyright file="GrapherMerger.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines members required for smart</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public void MergeGraph(IEditableBusinessObject target, IEditableBusinessObject source)
    {
      var imp = target as IManageProperties;
      if (imp != null)
      {
        var targetProperties = imp.GetManagedProperties();
        foreach (var item in targetProperties)
        {
          var oldValue = ReadProperty(source, item);
          var oldChild = oldValue as IEditableBusinessObject;
          if (oldChild != null)
          {
            var targetValue = ReadProperty(target, item) as IEditableBusinessObject;
            if (targetValue != null)
              MergeGraph(targetValue, oldChild);
            else
              LoadProperty(target, item, oldChild);
          }
          else
          {
            var oldList = oldValue as IEditableCollection;
            if (oldList != null)
            {
              var targetList = ReadProperty(target, item) as IEditableCollection;
              MergeGraph(targetList, oldList);
            }
            else
            {
              LoadProperty(target, item, oldValue);
            }
          }
        }
        if (source.IsNew)
          MarkNew(target);
        else if (!source.IsDirty)
          MarkOld(target);
        CheckRules(target);
      }
    }

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    private void MergeGraph(IEditableCollection target, IEditableCollection source)
    {
      var listType = target.GetType();
      var childType = Utilities.GetChildItemType(listType);
      Reflection.MethodCaller.CallGenericMethod(
        this, "MergeBusinessListGraph", 
        new [] { listType, childType }, true, target, source);
    }

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    public void MergeBusinessListGraph<T,C>(T target, T source)
      where T : BusinessListBase<T,C> 
      where C : BusinessBase<C>, IBusinessObject
    {
      var deleted = new List<C>();
      foreach (var item in target)
      {
        var sourceItem = source.Where(_ => _.Identity == item.Identity).FirstOrDefault();
        if (sourceItem != null)
          MergeGraph(item, sourceItem);
        else
          deleted.Add(item);
      }

      // add items not in target
      foreach (var item in source)
        if (target.Count(_ => _.Identity == item.Identity) == 0)
          target.Add(item);

      // remove items not in source
      foreach (var item in deleted)
        target.Remove(item);
      GetDeletedList<C>(target).Clear();
    }
  }
}
