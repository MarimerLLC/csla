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
    public void Save()
    {
      base.DoSave();
    }

    /// <summary>
    /// Cancels changes made to the model 
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public void Cancel()
    {
      base.DoCancel();
    }

    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    public void AddNew()
    {
      base.DoAddNew();
    }

    /// <summary>
    /// Removes an item from the Model (if it
    /// is a collection).
    /// </summary>
    public void Remove(T item)
    {
      base.DoRemove(item);
    }

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    public void Delete()
    {
      base.DoDelete();
    }

    #endregion
  }
}
