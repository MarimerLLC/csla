using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Csla.Silverlight
{
  /// <summary>
  /// Base class used to create ViewModel objects,
  /// with pre-existing verbs for use by
  /// InvokeMethod or Invoke.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  public abstract class ViewModel<T> : ViewModelBase<T>
  {
    #region Verbs

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public virtual void Save(object sender, ExecuteEventArgs e)
    {
      Save();
    }

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public virtual void Save()
    {
      DoSave();
    }

    /// <summary>
    /// Cancels changes made to the model 
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public virtual void Cancel(object sender, ExecuteEventArgs e)
    {
      Cancel();
    }

    /// <summary>
    /// Cancels changes made to the model 
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public virtual void Cancel()
    {
      DoCancel();
    }

    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    public virtual void AddNew(object sender, ExecuteEventArgs e)
    {
      AddNew();
    }

    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    public virtual void AddNew()
    {
      DoAddNew();
    }

    /// <summary>
    /// Removes an item from the Model (if it
    /// is a collection).
    /// </summary>
    public virtual void Remove(T item)
    {
      DoRemove(item);
    }

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    public virtual void Delete(object sender, ExecuteEventArgs e)
    {
      Delete();
    }

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    public virtual void Delete()
    {
      DoDelete();
    }

    #endregion
  }
}
