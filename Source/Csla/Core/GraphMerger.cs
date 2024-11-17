//-----------------------------------------------------------------------
// <copyright file="GrapherMerger.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines members required for smart</summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Csla.Core
{
  /// <summary>
  /// Implements behavior to merge one object graph
  /// into a clone of itself (typically post-serialization).
  /// </summary>
  public class GraphMerger : Server.ObjectFactory
  {
    private static readonly ConcurrentDictionary<(Type, Type, bool), System.Reflection.MethodInfo> _methodCache = new();
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// 
    public GraphMerger(ApplicationContext applicationContext)
      : base(applicationContext) { }

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public void MergeGraph(IEditableBusinessObject target, IEditableBusinessObject source)
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      if (target is IManageProperties imp)
      {
        FieldManager.FieldDataManager? targetFieldManager = null;
        if (target is IUseFieldManager iufm)
          targetFieldManager = iufm.FieldManager;
        FieldManager.FieldDataManager? sourceFieldManager = null;
        if (source is IUseFieldManager iufms)
          sourceFieldManager = iufms.FieldManager;

        var targetProperties = imp.GetManagedProperties();
        foreach (var item in targetProperties)
        {
          var sourceFieldExists = true;
          if (sourceFieldManager != null && (item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
            sourceFieldExists = sourceFieldManager.FieldExists(item);
          object? sourceValue = null;
          if (sourceFieldExists)
            sourceValue = ReadProperty(source, item);
          if (sourceValue is IEditableBusinessObject sourceChild)
          {
            var targetFieldExists = true;
            if (targetFieldManager != null && (item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
              targetFieldExists = targetFieldManager.FieldExists(item);
            if (targetFieldExists && ReadProperty(target, item) is IEditableBusinessObject targetChild)
            {
              MergeGraph(targetChild, sourceChild);
            }
            else
            {
              if ((item.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
                Reflection.MethodCaller.CallPropertySetter(target, item.Name, sourceChild);
              else
                LoadProperty(target, item, sourceChild);
            }
          }
          else
          {
            if (sourceValue is IEditableCollection sourceList)
            {
              var targetFieldExists = true;
              if (targetFieldManager != null && (item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
                targetFieldExists = targetFieldManager.FieldExists(item);
              if (targetFieldExists && ReadProperty(target, item) is IEditableCollection targetList)
              {
                MergeGraph(targetList, sourceList);
              }
              else
              {
                if ((item.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
                  Reflection.MethodCaller.CallPropertySetter(target, item.Name, sourceList);
                else
                  LoadProperty(target, item, sourceList);
              }
            }
            else
            {
              if ((item.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
                Reflection.MethodCaller.CallPropertySetter(target, item.Name, sourceValue);
              else if (sourceValue != null || (item.RelationshipType & RelationshipTypes.LazyLoad) != RelationshipTypes.LazyLoad)
                LoadProperty(target, item, sourceValue);
            }
          }
        }
        if (source.IsNew)
        {
          MarkNew(target);
        }
        else if (!source.IsDirty)
        {
          MarkOld(target);
        }
        else
        {
          CopyField(source, target, "_isDirty");
          CopyField(source, target, "_isNew");
          CopyField(source, target, "_isDeleted");
        }
        CheckRules(target);
      }
    }

    private static void CopyField(object? source, object? target, string fieldName)
    {
      if (source == null) return;
      if (target == null) return;
      var sourceField = source.GetType().GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
      if (sourceField != null)
      {
        var targetField = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (targetField != null)
        {
          targetField.SetValue(target, sourceField.GetValue(source));
        }
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
      var childType = Utilities.GetChildItemType(listType) ?? throw new InvalidOperationException();
      var genericTypeParams = new Type[] { listType, childType };
      System.Reflection.MethodInfo? methodReference;
      if (typeof(IExtendedBindingList).IsAssignableFrom(listType))
        methodReference = GetType().GetMethod("MergeBusinessBindingListGraph");
      else
        methodReference = GetType().GetMethod("MergeBusinessListGraph");

      var gr = methodReference!.MakeGenericMethod(genericTypeParams);
      gr.Invoke(this, [target, source]);
    }

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public void MergeBusinessListGraph<T, C>(T target, T source)
      where T : BusinessListBase<T, C>
      where C : IEditableBusinessObject
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      var deleted = new List<C>();
      foreach (var item in target)
      {
        var sourceItem = source.FirstOrDefault(_ => _.Identity == item.Identity);
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

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public void MergeBusinessBindingListGraph<T, C>(T target, T source)
      where T : BusinessBindingListBase<T, C>
      where C : IEditableBusinessObject
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      var deleted = new List<C>();
      foreach (var item in target)
      {
        var sourceItem = source.FirstOrDefault(_ => _.Identity == item.Identity);
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

    #region Async Methods

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public async Task MergeGraphAsync(IEditableBusinessObject target, IEditableBusinessObject source)
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      if (target is IManageProperties imp)
      {
        FieldManager.FieldDataManager? targetFieldManager = null;
        if (target is IUseFieldManager iufm)
          targetFieldManager = iufm.FieldManager;
        FieldManager.FieldDataManager? sourceFieldManager = null;
        if (source is IUseFieldManager iufms)
          sourceFieldManager = iufms.FieldManager;

        var targetProperties = imp.GetManagedProperties();
        foreach (var item in targetProperties)
        {
          var sourceFieldExists = true;
          if (sourceFieldManager != null && (item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
            sourceFieldExists = sourceFieldManager.FieldExists(item);
          object? sourceValue = null;
          if (sourceFieldExists)
            sourceValue = ReadProperty(source, item);
          if (sourceValue is IEditableBusinessObject sourceChild)
          {
            var targetFieldExists = true;
            if (targetFieldManager != null && (item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
              targetFieldExists = targetFieldManager.FieldExists(item);
            if (targetFieldExists && ReadProperty(target, item) is IEditableBusinessObject targetChild)
            {
              await MergeGraphAsync(targetChild, sourceChild);
            }
            else
            {
              if ((item.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
                Reflection.MethodCaller.CallPropertySetter(target, item.Name, sourceChild);
              else
                LoadProperty(target, item, sourceChild);
            }
          }
          else
          {
            if (sourceValue is IEditableCollection sourceList)
            {
              var targetFieldExists = true;
              if (targetFieldManager != null && (item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
                targetFieldExists = targetFieldManager.FieldExists(item);
              if (targetFieldExists && ReadProperty(target, item) is IEditableCollection targetList)
              {
                await MergeGraphAsync(targetList, sourceList);
              }
              else
              {
                if ((item.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
                  Reflection.MethodCaller.CallPropertySetter(target, item.Name, sourceList);
                else
                 LoadProperty(target, item, sourceList);
              }
            }
            else
            {
              if ((item.RelationshipType & RelationshipTypes.PrivateField) == RelationshipTypes.PrivateField)
                Reflection.MethodCaller.CallPropertySetter(target, item.Name, sourceValue);
              else if (sourceValue != null || (item.RelationshipType & RelationshipTypes.LazyLoad) != RelationshipTypes.LazyLoad)
                LoadProperty(target, item, sourceValue);
            }
          }
        }
        if (source.IsNew)
        {
          MarkNew(target);
        }
        else if (!source.IsDirty)
        {
          MarkOld(target);
        }
        else
        {
          CopyField(source, target, "_isDirty");
          CopyField(source, target, "_isNew");
          CopyField(source, target, "_isDeleted");
        }
        await CheckRulesAsync(target);
      }
    }

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    private async Task MergeGraphAsync(IEditableCollection target, IEditableCollection source)
    {
      var cacheKey = GetCacheKey(target);

      var methodReference = _methodCache.GetOrAdd(cacheKey, ((Type ListType, Type ChildType, bool IsExtendedBindingList) key) =>
      {
        var methodName = key.IsExtendedBindingList ? "MergeBusinessBindingListGraphAsync" : "MergeBusinessListGraphAsync";
        return GetType().GetMethod(methodName)!.MakeGenericMethod(key.ListType, key.ChildType);
      });

      var task = (Task)methodReference.Invoke(this, [target, source])!;
      await task;

      static (Type ListType, Type ChildType, bool IsExtendedBindingList) GetCacheKey(IEditableCollection target)
      {
        var listType = target.GetType();
        var childType = Utilities.GetChildItemType(listType) ?? throw new InvalidOperationException();
        var isExtendedBindingList = typeof(IExtendedBindingList).IsAssignableFrom(listType);
        return (listType, childType, isExtendedBindingList);
      }
    }
    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public async Task MergeBusinessListGraphAsync<T, C>(T target, T source)
      where T : BusinessListBase<T, C>
      where C : IEditableBusinessObject
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      var deleted = new List<C>();
      foreach (var item in target)
      {
        var sourceItem = source.FirstOrDefault(_ => _.Identity == item.Identity);
        if (sourceItem != null)
          await MergeGraphAsync(item, sourceItem);
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

    /// <summary>
    /// Merges state from source graph into target graph.
    /// </summary>
    /// <param name="target">Target of merge.</param>
    /// <param name="source">Source for merge.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public async Task MergeBusinessBindingListGraphAsync<T, C>(T target, T source)
      where T : BusinessBindingListBase<T, C>
      where C : IEditableBusinessObject
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      var deleted = new List<C>();
      foreach (var item in target)
      {
        var sourceItem = source.FirstOrDefault(_ => _.Identity == item.Identity);
        if (sourceItem != null)
          await MergeGraphAsync(item, sourceItem);
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

    #endregion Async Methods
  }
}
