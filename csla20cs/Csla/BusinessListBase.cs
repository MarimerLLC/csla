using System;
using System.ComponentModel;
using System.Collections.Generic;
using Csla.Properties;

namespace Csla
{
/*  Public MustInherit Class BusinessListBase(Of T As BusinessListBase(Of T, C), C As Core.BusinessBase)
  Inherits System.ComponentModel.BindingList(Of C)

  Implements Core.IEditableCollection
  Implements ICloneable
*/
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class BusinessListBase<T, C> : System.ComponentModel.BindingList<C>,
      Core.IEditableCollection, ICloneable
    where T : BusinessListBase<T, C>
    where C : Core.BusinessBase
  {

    #region Constructors

    protected BusinessListBase()
    {

    }

    #endregion

    #region IsDirty, IsValid

    /// <summary>
    /// Returns True if this object's data has been changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an object's data is changed, CSLA .NET makes note of that change
    /// and considers the object to be 'dirty' or changed. This value is used to
    /// optimize data updates, since an unchanged object does not need to be
    /// updated into the database. All new objects are considered dirty. All objects
    /// marked for deletion are considered dirty.
    /// </para><para>
    /// Once an object's data has been saved to the database (inserted or updated)
    /// the dirty flag is cleared and the object is considered unchanged. Objects
    /// newly loaded from the database are also considered unchanged.
    /// </para>
    /// <para>
    /// If any child object within the collection is dirty then the collection
    /// is considered to be dirty. If all child objects are unchanged, then the
    /// collection is not dirty.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    public bool IsDirty
    {
      get
      {
        // any deletions make us dirty
        if (DeletedList.Count > 0) return true;

        // run through all the child objects
        // and if any are dirty then then
        // collection is dirty
        foreach (C child in this)
          if (child.IsDirty)
            return true;
        return false;
      }
    }

    /// <summary>
    /// Returns True if the object is currently valid, False if the
    /// object has broken rules or is otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling <see cref="Validation.ValidationRules" />
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. For instance, you should always override
    /// this method if your object has child objects, since the validity of this object
    /// is affected by the validity of all child objects.
    /// </para>
    /// <para>
    /// If any child object within the collection is invalid then the collection
    /// is considered to be invalid. If all child objects are valid, then the
    /// collection is valid.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    public virtual bool IsValid
    {
      get
      {
        // run through all the child objects
        // and if any are invalid then the
        // collection is invalid
        foreach (C child in this)
          if (!child.IsValid)
            return false;
        return true;
      }
    }

    #endregion

    #region Begin/Cancel/ApplyEdit

    /// <summary>
    /// Starts a nested edit on the object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this method is called the object takes a snapshot of
    /// its current state (the values of its variables). This snapshot
    /// can be restored by calling <see cref="Csla.Core.BusinessBase.CancelEdit" />
    /// or committed by calling <see cref="Csla.Core.BusinessBase.ApplyEdit" />.
    /// </para><para>
    /// This is a nested operation. Each call to BeginEdit adds a new
    /// snapshot of the object's state to a stack. You should ensure that 
    /// for each call to BeginEdit there is a corresponding call to either 
    /// CancelEdit or ApplyEdit to remove that snapshot from the stack.
    /// </para><para>
    /// See Chapters 2 and 4 for details on n-level undo and state stacking.
    /// </para><para>
    /// This method triggers the copying of all child object states.
    /// </para>
    /// </remarks>
    public void BeginEdit()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoBeginEditChildException);

      CopyState();
    }

    /// <summary>
    /// Cancels the current edit process, restoring the object's state to
    /// its previous values.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be restored. This resets the object's values
    /// to the point of the last <see cref="Csla.BusinessCollectionBase.BeginEdit" />
    /// call.
    /// <para>
    /// This method triggers an undo in all child objects.
    /// </para>
    /// </remarks>
    public void CancelEdit()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoCancelEditChildException);

      UndoChanges();

      // make sure the child objects re-add their business rules
      foreach (C child in this)
      {
        child.SetParent(this);
        child.AddBusinessRules();
      }

      foreach (C child in DeletedList)
      {
        child.SetParent(this);
        child.AddBusinessRules();
      }
    }

    /// <summary>
    /// Commits the current edit process.
    /// </summary>
    /// <remarks>
    /// Calling this method causes the most recently taken snapshot of the 
    /// object's state to be discarded, thus committing any changes made
    /// to the object's state since the last 
    /// <see cref="Csla.BusinessCollectionBase.BeginEdit" /> call.
    /// <para>
    /// This method triggers an ApplyEdit in all child objects.
    /// </para>
    /// </remarks>
    public void ApplyEdit()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoApplyEditChildException);

      AcceptChanges();
    }

    #endregion

    #region N-level undo

    void Core.IEditableObject.CopyState()
    {
      CopyState();
    }

    void Core.IEditableObject.UndoChanges()
    {
      UndoChanges();
    }

    void Core.IEditableObject.AcceptChanges()
    {
      AcceptChanges();
    }

    private void CopyState()
    {
      // we are going a level deeper in editing
      _editLevel += 1;

      // cascade the call to all child objects
      foreach (C child in this)
        child.CopyState();

      // cascade the call to all deleted child objects
      foreach (C child in DeletedList)
        child.CopyState();
    }

    private void UndoChanges()
    {
      C child;

      // we are coming up one edit level
      _editLevel -= 1;
      if (_editLevel < 0) _editLevel = 0;

      // Cancel edit on all current items
      for (int index = Count - 1; index >= 0; index--)
      {
        child = this[index];
        child.UndoChanges();
        // if item is below its point of addition, remove
        if (child.EditLevelAdded > _editLevel)
          RemoveAt(index);
      }

      // cancel edit on all deleted items
      for (int index = DeletedList.Count - 1; index >= 0; index--)
      {
        child = DeletedList[index];
        child.UndoChanges();
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

    private void AcceptChanges()
    {
      // we are coming up one edit level
      _editLevel -= 1;
      if (_editLevel < 0) _editLevel = 0;

      // cascade the call to all child objects
      foreach (C child in this)
      {
        child.AcceptChanges();
        // if item is below its point of addition, lower point of addition
        if (child.EditLevelAdded > _editLevel) child.EditLevelAdded = _editLevel;
      }

      // cascade the call to all deleted child objects
      for (int index = DeletedList.Count - 1; index >= 0; index--)
      {
        C child = DeletedList[index];
        child.AcceptChanges();
        // if item is below its point of addition, remove
        if (child.EditLevelAdded > _editLevel)
          DeletedList.RemoveAt(index);
      }
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
      // mark the object as deleted
      child.DeleteChild();
      // and add it to the deleted collection for storage
      DeletedList.Add(child);
    }

    private void UnDeleteChild(C child)
    {
      // we are inserting an _existing_ object so
      // we need to preserve the object's editleveladded value
      // because it will be changed by the normal add process
      int saveLevel = child.EditLevelAdded;
      Add(child);
      child.EditLevelAdded = saveLevel;

      // since the object is no longer deleted, remove it from
      // the deleted collection
      DeletedList.Remove(child);
    }

    /// <summary>
    /// Returns True if the internal deleted list
    /// contains the specified child object.
    /// </summary>
    /// <param name="item">Child object to check.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool ContainsDeleted(C item)
    {
      return DeletedList.Contains(item);
    }

    #endregion

    #region Insert, Remove, Clear

    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    void Core.IEditableCollection.RemoveChild(Csla.Core.BusinessBase child)
    {
      Remove((C)child);
    }

    /// <summary>
    /// Sets the edit level of the child object as it is added.
    /// </summary>
    protected override void InsertItem(int index, C item)
    {
      // when an object is inserted we assume it is
      // a new object and so the edit level when it was
      // added must be set
      item.EditLevelAdded = _editLevel;
      item.SetParent(this);
      base.InsertItem(index, item);
    }

    /// <summary>
    /// Marks the child object for deletion and moves it to
    /// the collection of deleted objects.
    /// </summary>
    protected override void RemoveItem(int index)
    {
      // when an object is 'removed' it is really
      // being deleted, so do the deletion work
      DeleteChild(this[index]);
      base.RemoveItem(index);
    }

    #endregion

    #region Edit level tracking

    // keep track of how many edit levels we have
    private int _editLevel;

    #endregion

    #region IsChild

    [NotUndoable()]
    private bool _isChild = false;

    /// <summary>
    /// Indicates whether this collection object is a child object.
    /// </summary>
    /// <returns>True if this is a child object.</returns>
    protected bool IsChild
    {
      get { return _isChild; }
    }

    /// <summary>
    /// Marks the object as being a child object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default all business objects are 'parent' objects. This means
    /// that they can be directly retrieved and updated into the database.
    /// </para><para>
    /// We often also need child objects. These are objects which are contained
    /// within other objects. For instance, a parent Invoice object will contain
    /// child LineItem objects.
    /// </para><para>
    /// To create a child object, the MarkAsChild method must be called as the
    /// object is created. Please see Chapter 7 for details on the use of the
    /// MarkAsChild method.
    /// </para>
    /// </remarks>
    protected void MarkAsChild()
    {
      _isChild = true;
    }

    #endregion

    #region ICloneable

    object ICloneable.Clone()
    {
      return OnClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object OnClone()
    {
      return Core.ObjectCloner.Clone(this);
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public T Clone()
    {
      return (T)OnClone();
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Saves the object to the database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calling this method starts the save operation, causing the all child
    /// objects to be inserted, updated or deleted within the database based on the
    /// each object's current state.
    /// </para><para>
    /// All this is contingent on <see cref="Csla.BusinessCollectionBase.IsDirty" />. If
    /// this value is False, no data operation occurs. It is also contingent on
    /// <see cref="Csla.BusinessCollectionBase.IsValid" />. If this value is False an
    /// exception will be thrown to indicate that the UI attempted to save an
    /// invalid object.
    /// </para><para>
    /// It is important to note that this method returns a new version of the
    /// business collection that contains any data updated during the save operation.
    /// You MUST update all object references to use this new version of the
    /// business collection in order to have access to the correct object data.
    /// </para><para>
    /// You can override this method to add your own custom behaviors to the save
    /// operation. For instance, you may add some security checks to make sure
    /// the user can save the object. If all security checks pass, you would then
    /// invoke the base Save method via <c>MyBase.Save()</c>.
    /// </para>
    /// </remarks>
    /// <returns>A new object containing the saved values.</returns>
    public virtual T Save()
    {
      if (this.IsChild)
        throw new NotSupportedException(Resources.NoSaveChildException);

      if (_editLevel > 0)
        throw new Validation.ValidationException(Resources.NoSaveEditingException);

      if (!IsValid)
        throw new Validation.ValidationException(Resources.NoSaveInvalidException);

      if (IsDirty)
        return (T)DataPortal.Update(this);
      else
        return (T)this;
    }

    /// <summary>
    /// Override this method to load a new business object with default
    /// values from the database.
    /// </summary>
    /// <param name="Criteria">An object containing criteria values.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="Criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow update of a business
    /// object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow immediate deletion of a business object.
    /// </summary>
    /// <param name="Criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    #endregion
  }
}