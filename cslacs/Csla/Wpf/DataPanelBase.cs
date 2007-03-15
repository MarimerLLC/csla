using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;

namespace Csla.Wpf
{
  /// <summary>
  /// Base class for creating WPF panel
  /// controls that react when the DataContext,
  /// data object and data property values
  /// are changed.
  /// </summary>
  public class DataPanelBase : Decorator
  {
    private bool _loaded;
    private object _dataObject;

    /// <summary>
    /// Gets a reference to the current
    /// data object.
    /// </summary>
    /// <remarks>
    /// The DataContext may not be the data object. The
    /// DataContext may be a DataSourceProvider control.
    /// This property returns a reference to the actual
    /// <b>data object</b>, not necessarily the DataContext
    /// itself.
    /// </remarks>
    protected object DataObject
    {
      get
      {
        return _dataObject;
      }
    }

    /// <summary>
    /// This method is called when a property
    /// of the data object to which the 
    /// control is bound has changed.
    /// </summary>
    protected virtual void DataPropertyChanged(PropertyChangedEventArgs e)
    {
      // may be overridden by subclass
    }

    /// <summary>
    /// This method is called when the data
    /// object to which the control is bound
    /// has changed.
    /// </summary>
    protected virtual void DataObjectChanged()
    {
      // may be overridden by subclass
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public DataPanelBase()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(Panel_DataContextChanged);
      this.Loaded += new RoutedEventHandler(Panel_Loaded);
    }

    /// <summary>
    /// Handle case where the DataContext for the
    /// control has changed.
    /// </summary>
    private void Panel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      UnHookDataContextEvents(e.OldValue);

      // store a ref to the data object
      _dataObject = GetDataObject(e.NewValue);

      HookDataContextEvents(e.NewValue);

      if (_loaded)
        DataObjectChanged();
    }

    private object GetDataObject(object dataContext)
    {
      DataSourceProvider provider = dataContext as DataSourceProvider;
      if (provider != null)
        return provider.Data;
      else
        return dataContext;
    }

    /// <summary>
    /// Handle case where the Data property of the
    /// DataContext (a DataSourceProvider) has changed.
    /// </summary>
    private void DataProvider_DataChanged(object sender, EventArgs e)
    {
      UnHookPropertyChanged(_dataObject as INotifyPropertyChanged);

      _dataObject = ((DataSourceProvider)sender).Data as IDataErrorInfo;

      HookPropertyChanged(_dataObject as INotifyPropertyChanged);

      DataObjectChanged();
    }

    private void UnHookDataContextEvents(object oldValue)
    {
      // unhook any old event handling
      INotifyPropertyChanged oldContext = null;

      DataSourceProvider provider = oldValue as DataSourceProvider;
      if (provider == null)
      {
        oldContext = oldValue as INotifyPropertyChanged;
      }
      else
      {
        provider.DataChanged -= new EventHandler(DataProvider_DataChanged);
        oldContext = provider.Data as INotifyPropertyChanged;
      }
      UnHookPropertyChanged(oldContext);
    }

    private void HookDataContextEvents(object newValue)
    {
      // hook any new event
      INotifyPropertyChanged newContext = null;

      DataSourceProvider provider = newValue as DataSourceProvider;
      if (provider == null)
      {
        newContext = newValue as INotifyPropertyChanged;
      }
      else
      {
        provider.DataChanged += new EventHandler(DataProvider_DataChanged);
        newContext = provider.Data as INotifyPropertyChanged;
      }
      HookPropertyChanged(newContext);
    }

    private void UnHookPropertyChanged(INotifyPropertyChanged oldContext)
    {
      if (oldContext != null)
        oldContext.PropertyChanged -= new PropertyChangedEventHandler(DataObject_PropertyChanged);
    }

    private void HookPropertyChanged(INotifyPropertyChanged newContext)
    {
      if (newContext != null)
        newContext.PropertyChanged += new PropertyChangedEventHandler(DataObject_PropertyChanged);
    }

    private void Panel_Loaded(object sender, RoutedEventArgs e)
    {
      _loaded = true;
      if (_dataObject != null)
        DataObjectChanged();
    }

    private void DataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      DataPropertyChanged(e);
    }
  }
}
