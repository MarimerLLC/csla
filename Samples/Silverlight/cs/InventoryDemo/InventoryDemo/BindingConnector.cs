using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Core;
using Csla.Silverlight;

namespace InventoryDemo
{
  public class BindingConnector : Control
  {
    public static DependencyProperty ObjectInstanceProperty =
      DependencyProperty.Register("ObjectInstance", typeof(object), typeof(BindingConnector),
      new PropertyMetadata((o, e) => 
      {
        var dp = ((BindingConnector)o).DataProvider;
        if (dp != null)
          dp.ObjectInstance = e.NewValue;
      }));

    public object ObjectInstance
    {
      get
      { 
        return ((object)(base.GetValue(ObjectInstanceProperty)));
      }
      set
      {
        base.SetValue(ObjectInstanceProperty, value);
      }
    }

    public static DependencyProperty DataProviderProperty =
      DependencyProperty.Register("DataProvider", typeof(CslaDataProvider), typeof(BindingConnector),
      new PropertyMetadata((o, e) =>
      {
        var dp = e.NewValue as CslaDataProvider;
        if (dp != null)
          dp.ObjectInstance = ((BindingConnector)o).ObjectInstance;
      }));

    public CslaDataProvider DataProvider
    {
      get
      {
        return ((CslaDataProvider)(base.GetValue(DataProviderProperty)));
      }
      set
      {
        base.SetValue(DataProviderProperty, value);
      }
    }

    //public bool CanCancel
    //{
    //  get 
    //  {
    //    ITrackStatus targetObject = this.ObjectInstance as ITrackStatus;
    //    if (targetObject != null && Csla.Security.AuthorizationRules.CanEditObject(this.ObjectInstance.GetType()) && targetObject.IsDirty) // && !isObjectBusy)
    //      return true;
    //    else
    //      return false;
    //  }
    //}

    ///// <summary>
    ///// Cancels any changes to the object.
    ///// </summary>
    //public void Cancel()
    //{
    //  Exception _error = null;
    //  try
    //  {
    //    var undoable = ObjectInstance as Csla.Core.ISupportUndo;
    //    if (undoable != null)
    //    {
    //      //IsBusy = true;
    //      ObjectInstance = null;
    //      undoable.CancelEdit();
    //      ObjectInstance = undoable;
    //      var trackable = ObjectInstance as ITrackStatus;
    //      //if (trackable != null)
    //      //  IsBusy = trackable.IsBusy;
    //      //else
    //      //  IsBusy = false;
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    _error = ex;
    //  }
    //}
  }
}
