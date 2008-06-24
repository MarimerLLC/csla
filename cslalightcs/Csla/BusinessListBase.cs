using System;
using System.ComponentModel;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core;
using Csla.Properties;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Csla
{
  [Serializable]
  public class BusinessListBase<T, C> : Core.ExtendedBindingList<C>, ICloneable, IUndoableObject
    where T: BusinessListBase<T, C>
    where C : Core.IEditableBusinessObject
  {
    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return Core.ObjectCloner.Clone(this);
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public T Clone()
    {
      return (T)GetClone();
    }

    #endregion

    #region Delete and Undelete child

    private List<C> _deletedList;

    /// <summary>
    /// A collection containing all child objects marked
    /// for deletion.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected List<C> DeletedList
    {
      get
      {
        if (_deletedList == null)
          _deletedList = new List<C>();
        return _deletedList;
      }
    }

    private void DeleteChild(C child)
    {
      // set child edit level
      Core.UndoableBase.ResetChildEditLevel(child, this.EditLevel, false);

      // TODO: Implement i4o
      // remove from the index
      //RemoveIndexItem(child);
      // remove from the position map
      //RemoveFromMap(child);

      // mark the object as deleted
      child.DeleteChild();
      // and add it to the deleted collection for storage
      DeletedList.Add(child);
    }

    private void UnDeleteChild(C child)
    {
      // since the object is no longer deleted, remove it from
      // the deleted collection
      DeletedList.Remove(child);

      // we are inserting an _existing_ object so
      // we need to preserve the object's editleveladded value
      // because it will be changed by the normal add process
      int saveLevel = child.EditLevelAdded;
      
      // TODO: implement i4o
      //InsertIndexItem(child);

      Add(child);
      child.EditLevelAdded = saveLevel;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the internal deleted list
    /// contains the specified child object.
    /// </summary>
    /// <param name="item">Child object to check.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool ContainsDeleted(C item)
    {
      return DeletedList.Contains(item);
    }

    #endregion

    #region Edit level tracking

    // keep track of how many edit levels we have
    private int _editLevel;

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int EditLevel
    {
      get { return _editLevel; }
    }

    int Core.IUndoableObject.EditLevel
    {
      get
      {
        return this.EditLevel;
      }
    }

    #endregion

    #region N-level undo

    void Core.IUndoableObject.CopyState(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        CopyState(parentEditLevel);
    }

    void Core.IUndoableObject.UndoChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        UndoChanges(parentEditLevel);
    }

    void Core.IUndoableObject.AcceptChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        AcceptChanges(parentEditLevel);
    }

    private void CopyState(int parentEditLevel)
    {
      if (this.EditLevel + 1 > parentEditLevel)
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"));

      // we are going a level deeper in editing
      _editLevel += 1;

      // JMC 6/24/08
      // This used to be a foreach loop but there appears to be a bug
      // in the silverlight runtime (SL2 B2) since calling foreach here will result
      // in SEHException with the error code -2147467259, or Error Unkown.
      // Iterating on this collection outside of this call will result in
      // behavior as expected but for some reason doing it here results in
      // an unknown exception.

      // cascade the call to all child objects
      for (int x = 0; x < this.Count; x++)
      {
        C child = this[x];
        child.CopyState(_editLevel, false);
      }

      // cascade the call to all deleted child objects
      foreach (C child in DeletedList)
        child.CopyState(_editLevel, false);
    }

    private bool _completelyRemoveChild;

    private void UndoChanges(int parentEditLevel)
    {
      C child;

      if (this.EditLevel - 1 < parentEditLevel)
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"));

      // we are coming up one edit level
      _editLevel -= 1;
      if (_editLevel < 0) _editLevel = 0;

      bool oldRLCE = this.RaiseListChangedEvents;
      this.RaiseListChangedEvents = false;
      try
      {
        // Cancel edit on all current items
        for (int index = Count - 1; index >= 0; index--)
        {
          child = this[index];

          //ACE: Important, make sure to remove the item prior to
          //     it going through undo, otherwise, it will
          //     incur a more expensive RemoveByReference operation
          //DeferredLoadIndexIfNotLoaded();
          //_indexSet.RemoveItem(child);

          child.UndoChanges(_editLevel, false);

          //ACE: Now that we have undone the changes, we can add the item
          //     back in the index.
          //_indexSet.InsertItem(child);

          // if item is below its point of addition, remove
          if (child.EditLevelAdded > _editLevel)
          {
            bool oldAllowRemove = this.AllowRemove;
            try
            {
              this.AllowRemove = true;
              _completelyRemoveChild = true;
              //RemoveIndexItem(child);
              RemoveAt(index);
            }
            finally
            {
              _completelyRemoveChild = false;
              this.AllowRemove = oldAllowRemove;
            }
          }
        }

        // cancel edit on all deleted items
        for (int index = DeletedList.Count - 1; index >= 0; index--)
        {
          child = DeletedList[index];
          child.UndoChanges(_editLevel, false);
          if (child.EditLevelAdded > _editLevel)
          {
            // if item is below its point of addition, remove
            DeletedList.RemoveAt(index);
          }
          else
          {
            // if item is no longer deleted move back to main list
            if (!child.IsDeleted) UnDeleteChild(child);
          }
        }
      }
      finally
      {
        this.RaiseListChangedEvents = oldRLCE;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }

    private void AcceptChanges(int parentEditLevel)
    {
      if (this.EditLevel - 1 < parentEditLevel)
        throw new Core.UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"));

      // we are coming up one edit level
      _editLevel -= 1;
      if (_editLevel < 0) _editLevel = 0;

      // cascade the call to all child objects
      foreach (C child in this)
      {
        child.AcceptChanges(_editLevel, false);
        // if item is below its point of addition, lower point of addition
        if (child.EditLevelAdded > _editLevel) child.EditLevelAdded = _editLevel;
      }

      // cascade the call to all deleted child objects
      for (int index = DeletedList.Count - 1; index >= 0; index--)
      {
        C child = DeletedList[index];
        child.AcceptChanges(_editLevel, false);
        // if item is below its point of addition, remove
        if (child.EditLevelAdded > _editLevel)
          DeletedList.RemoveAt(index);
      }
    }

    #endregion
  }
}
