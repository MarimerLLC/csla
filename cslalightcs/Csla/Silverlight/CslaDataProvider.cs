using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Properties;

namespace Csla.Silverlight
{
  /// <summary>
  /// Creates, retrieves and manages business objects
  /// from XAML in a form.
  /// </summary>
  public class CslaDataProvider : INotifyPropertyChanged
  {
    private object _dataObject;

    /// <summary>
    /// Gets or sets a reference to the
    /// object containing the data for binding.
    /// </summary>
    public object Data 
    {
      get 
      { 
        return _dataObject; 
      }
      set
      {
        _dataObject = value;
        if (_manageObjectLifetime)
        {
          var undoable = _dataObject as Csla.Core.ISupportUndo;
          if (undoable != null)
            undoable.BeginEdit();
        }

        try
        {
          OnPropertyChanged(new PropertyChangedEventArgs("Data"));
        }
        catch (NullReferenceException ex)
        {
          // Silverlight seems to throw a meaningless null ref exception
          // and this is a workaround to ignore it
          var o = ex;
        } 
      }
    }

    private bool _manageObjectLifetime = true;

    /// <summary>
    /// Gets or sets a value indicating whether
    /// the business object's lifetime should
    /// be managed automatically.
    /// </summary>
    public bool ManageObjectLifetime
    {
      get { return _manageObjectLifetime; }
      set 
      {
        if (_dataObject != null)
          throw new NotSupportedException(Resources.ObjectNotNull);
        _manageObjectLifetime = value; 
      }
    }

    /// <summary>
    /// Async event handler to be called when a
    /// data portal create or fetch operation
    /// completes.
    /// </summary>
    /// <param name="sender">
    /// Data portal object completing the async operation.
    /// </param>
    /// <param name="e">
    /// Async data portal result object.
    /// </param>
    public void DataPortalMethodCompleted(object sender, IDataPortalResult e)
    {
      Data = e.Object;
    }

    private Exception _error;

    /// <summary>
    /// Gets a reference to the Exception object
    /// (if any) from the last data portal operation.
    /// </summary>
    public Exception Error
    {
      get { return _error; }
      private set
      {
        _error = value;
        OnPropertyChanged(new PropertyChangedEventArgs("Error"));
      }
    }

    /// <summary>
    /// Cancels any changes to the object.
    /// </summary>
    public void Cancel()
    {
      if (_manageObjectLifetime)
      {
        var undoable = _dataObject as Csla.Core.ISupportUndo;
        if (undoable != null)
        {
          undoable.CancelEdit();
          undoable.BeginEdit();
        }
      }
    }

    /// <summary>
    /// Accepts any changes to the object and
    /// invokes the object's BeginSave() method.
    /// </summary>
    public void Save()
    {
      Error = null;
      if (_manageObjectLifetime)
      {
        var undoable = _dataObject as Csla.Core.ISupportUndo;
        if (undoable != null)
          undoable.ApplyEdit();
      }
      var obj = _dataObject as Csla.Core.ISavable;
      if (obj != null)
      {
        obj.Saved += new EventHandler<Csla.Core.SavedEventArgs>(obj_Saved);
        obj.BeginSave();
      }
    }

    void obj_Saved(object sender, Csla.Core.SavedEventArgs e)
    {
      if (e.Error != null)
        Error = e.Error;
      else
        Data = e.NewObject;
    }

    /// <summary>
    /// Marks an editable root object for deletion.
    /// </summary>
    public void Delete()
    {
      var obj = _dataObject as Csla.Core.BusinessBase;
      if (obj != null && !obj.IsChild)
        obj.Delete();
    }

    /// <summary>
    /// Begins an async add new operation to add a 
    /// new item to an editable list business object.
    /// </summary>
    public void AddNewItem()
    {
      var obj = _dataObject as Csla.Core.IBindingList;
      if (obj != null)
        obj.AddNew();
    }

    /// <summary>
    /// Removes an item from an editable list
    /// business object.
    /// </summary>
    /// <param name="item">
    /// Reference to the child item to remove.
    /// </param>
    public void RemoveItem(object item)
    {
      var obj = _dataObject as System.Collections.IList;
      if (obj != null)
        obj.Remove(item);
    }
    
    #region INotifyPropertyChanged Members

    /// <summary>
    /// Event raised when a property of the
    /// object has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="e">
    /// Arguments for event.
    /// </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, e);
    }

    #endregion
  }
}
