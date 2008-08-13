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

namespace Csla.Silverlight
{
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
        var undoable = _dataObject as Csla.Core.ISupportUndo;
        if (undoable != null)
          undoable.BeginEdit();

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

    public void CreateCompleted(object sender, IDataPortalResult e)
    {
      Data = e.Object;
    }

    public void FetchCompleted(object sender, IDataPortalResult e)
    {
      Data = e.Object;
    }

    public void UpdateCompleted(object sender, IDataPortalResult e)
    {
      Data = e.Object;
    }

    public void DeleteCompleted(object sender, IDataPortalResult e)
    {
      Data = e.Object;
    }

    private Exception _error;

    public Exception Error
    {
      get { return _error; }
      private set
      {
        _error = value;
        OnPropertyChanged(new PropertyChangedEventArgs("Error"));
      }
    }

    public void Cancel()
    {
      var undoable = _dataObject as Csla.Core.ISupportUndo;
      if (undoable != null)
      {
        undoable.CancelEdit();
        undoable.BeginEdit();
      }
    }

    public void Save()
    {
      var undoable = _dataObject as Csla.Core.ISupportUndo;
      if (undoable != null)
        undoable.ApplyEdit();
      var obj = _dataObject as Csla.Core.ISavable;
      if (obj != null)
      {
        obj.Saved += new EventHandler<Csla.Core.SavedEventArgs>(obj_Saved);
        obj.Save();
      }
    }

    void obj_Saved(object sender, Csla.Core.SavedEventArgs e)
    {
      if (e.Error != null)
        Error = e.Error;
      else
      {
        var undoable = e.NewObject as Csla.Core.ISupportUndo;
        if (undoable != null)
          undoable.BeginEdit();
        Data = e.NewObject;
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, e);
    }

    #endregion
  }
}
