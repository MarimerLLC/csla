#if !NET20
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
  public class DataDecoratorBase : Decorator
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
    /// This method is called if the data
    /// object is an IBindingList, and the 
    /// ListChanged event was raised by
    /// the data object.
    /// </summary>
    protected virtual void DataBindingListChanged(ListChangedEventArgs e)
    {
      // may be overridden by subclass
    }

    /// <summary>
    /// This method is called if the data
    /// object is an INotifyCollectionChanged, 
    /// and the CollectionChanged event was 
    /// raised by the data object.
    /// </summary>
    protected virtual void DataObservableCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
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
    public DataDecoratorBase()
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
      object oldContext = null;

      DataSourceProvider provider = oldValue as DataSourceProvider;
      if (provider == null)
      {
        oldContext = oldValue;
      }
      else
      {
        provider.DataChanged -= new EventHandler(DataProvider_DataChanged);
        oldContext = provider.Data;
      }
      UnHookPropertyChanged(oldContext as INotifyPropertyChanged);
      UnHookBindingListChanged(oldContext as IBindingList);
      UnHookObservableListChanged(oldContext as INotifyCollectionChanged);
    }

    private void HookDataContextEvents(object newValue)
    {
      // hook any new event
      object newContext = null;

      DataSourceProvider provider = newValue as DataSourceProvider;
      if (provider == null)
      {
        newContext = newValue;
      }
      else
      {
        provider.DataChanged += new EventHandler(DataProvider_DataChanged);
        newContext = provider.Data;
      }
      HookPropertyChanged(newContext as INotifyPropertyChanged);
      HookBindingListChanged(newContext as IBindingList);
      HookObservableListChanged(newContext as INotifyCollectionChanged);
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

    private void UnHookBindingListChanged(IBindingList oldContext)
    {
      if (oldContext != null)
        oldContext.ListChanged -= new ListChangedEventHandler(DataObject_ListChanged);
    }

    private void HookBindingListChanged(IBindingList newContext)
    {
      if (newContext != null)
        newContext.ListChanged += new ListChangedEventHandler(DataObject_ListChanged);
    }

    private void UnHookObservableListChanged(INotifyCollectionChanged oldContext)
    {
      if (oldContext != null)
        oldContext.CollectionChanged -=
          new NotifyCollectionChangedEventHandler(DataObject_CollectionChanged);
    }

    private void HookObservableListChanged(INotifyCollectionChanged newContext)
    {
      if (newContext != null)
        newContext.CollectionChanged +=
          new NotifyCollectionChangedEventHandler(DataObject_CollectionChanged);
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

    private void DataObject_ListChanged(object sender, ListChangedEventArgs e)
    {
      DataBindingListChanged(e);
    }

    private void DataObject_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      DataObservableCollectionChanged(e);
    }

    /// <summary>
    /// Scans all child controls of this panel
    /// for object bindings, and calls
    /// <see cref="FoundBinding"/> for each
    /// binding found.
    /// </summary>
    protected void FindChildBindings()
    {
      FindBindings(this);
    }

    private void FindBindings(Visual visual)
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
      {
        Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
        MemberInfo[] sharedMembers = childVisual.GetType().GetMembers(
          BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        foreach (MemberInfo member in sharedMembers)
        {
          DependencyProperty prop = null;
          if (member.MemberType == MemberTypes.Field)
            prop = ((FieldInfo)member).GetValue(childVisual) as DependencyProperty;
          else if (member.MemberType == MemberTypes.Property)
            prop = ((PropertyInfo)member).GetValue(childVisual, null) as DependencyProperty;

          if (prop != null)
          {
            Binding bnd = BindingOperations.GetBinding(childVisual, prop);
            if (bnd != null && bnd.RelativeSource == null && bnd.Path != null && string.IsNullOrEmpty(bnd.ElementName))
              FoundBinding(bnd, (FrameworkElement)childVisual, prop);
          }
        }
        FindBindings(childVisual);
      }
    }

    /// <summary>
    /// Called by
    /// <see cref="FindChildBindings"/> each
    /// time an object binding is found.
    /// </summary>
    /// <param name="bnd">The Binding object.</param>
    /// <param name="control">The control containing the binding.</param>
    /// <param name="prop">The data bound DependencyProperty.</param>
    protected virtual void FoundBinding(Binding bnd, FrameworkElement control, DependencyProperty prop)
    {
    }
  }
}
#endif