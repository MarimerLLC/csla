using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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
  //public class DataDecoratorBase : Control
  //{
  //  private object _dataObject;

  //  /// <summary>
  //  /// Gets a reference to the current
  //  /// data object.
  //  /// </summary>
  //  /// <remarks>
  //  /// The DataContext may not be the data object. The
  //  /// DataContext may be a DataSourceProvider control.
  //  /// This property returns a reference to the actual
  //  /// <b>data object</b>, not necessarily the DataContext
  //  /// itself.
  //  /// </remarks>
  //  protected object DataObject
  //  {
  //    get
  //    {
  //      return _dataObject;
  //    }
  //    set
  //    {
  //      UpdateDataObject(_dataObject, value);
  //      if (_loaded)
  //        DataObjectChanged();
  //    }
  //  }

  //  /// <summary>
  //  /// This method is called when a property
  //  /// of the data object to which the 
  //  /// control is bound has changed.
  //  /// </summary>
  //  protected virtual void DataPropertyChanged(PropertyChangedEventArgs e)
  //  {
  //    // may be overridden by subclass
  //  }

  //  /// <summary>
  //  /// This method is called if the data
  //  /// object is an INotifyCollectionChanged, 
  //  /// and the CollectionChanged event was 
  //  /// raised by the data object.
  //  /// </summary>
  //  protected virtual void DataObservableCollectionChanged(NotifyCollectionChangedEventArgs e)
  //  {
  //  }

  //  /// <summary>
  //  /// This method is called when the data
  //  /// object to which the control is bound
  //  /// has changed.
  //  /// </summary>
  //  protected virtual void DataObjectChanged()
  //  {
  //    this.Loaded += new RoutedEventHandler(DataDecoratorBase_Loaded);
  //  }

  //  void DataDecoratorBase_Loaded(object sender, RoutedEventArgs e)
  //  {
  //    if (DataObject == null && DataContext != null)
  //      DataObject = DataContext;
  //  }

  //  /// <summary>
  //  /// Creates an instance of the object.
  //  /// </summary>
  //  public DataDecoratorBase()
  //  {
  //    this.Visibility = Visibility.Collapsed;
  //  }

  //  private void UpdateDataObject(object oldObject, object newObject)
  //  {
  //    if(oldObject!=null)
  //      UnHookDataContextEvents(oldObject);

  //    // store a ref to the data object
  //    _dataObject = newObject;

  //    HookDataContextEvents(newObject);
  //  }

  //  private void UnHookDataContextEvents(object oldValue)
  //  {
  //    // unhook any old event handling
  //    UnHookPropertyChanged(oldValue as INotifyPropertyChanged);
  //    UnHookObservableListChanged(oldValue as INotifyCollectionChanged);
  //  }

  //  private void HookDataContextEvents(object newValue)
  //  {
  //    // hook any new event
  //    HookPropertyChanged(newValue as INotifyPropertyChanged);
  //    HookObservableListChanged(newValue as INotifyCollectionChanged);
  //  }

  //  private void UnHookPropertyChanged(INotifyPropertyChanged oldContext)
  //  {
  //    if (oldContext != null)
  //      oldContext.PropertyChanged -= new PropertyChangedEventHandler(DataObject_PropertyChanged);
  //  }

  //  private void HookPropertyChanged(INotifyPropertyChanged newContext)
  //  {
  //    if (newContext != null)
  //      newContext.PropertyChanged += new PropertyChangedEventHandler(DataObject_PropertyChanged);
  //  }

  //  private void UnHookObservableListChanged(INotifyCollectionChanged oldContext)
  //  {
  //    if (oldContext != null)
  //      oldContext.CollectionChanged -=
  //        new NotifyCollectionChangedEventHandler(DataObject_CollectionChanged);
  //  }

  //  private void HookObservableListChanged(INotifyCollectionChanged newContext)
  //  {
  //    if (newContext != null)
  //      newContext.CollectionChanged +=
  //        new NotifyCollectionChangedEventHandler(DataObject_CollectionChanged);
  //  }

  //  private void DataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
  //  {
  //    DataPropertyChanged(e);
  //  }

  //  private void DataObject_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  //  {
  //    DataObservableCollectionChanged(e);
  //  }

  //  /// <summary>
  //  /// Scans all child controls of this panel
  //  /// for object bindings, and calls
  //  /// <see cref="FoundBinding"/> for each
  //  /// binding found.
  //  /// </summary>
  //  protected void FindChildBindings()
  //  {
  //    FindBindings(VisualTreeHelper.GetParent(this));
  //  }

  //  private void FindBindings(DependencyObject visual)
  //  {
  //    if (visual != this)
  //    {
  //      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
  //      {
  //        DependencyObject childVisual = (DependencyObject)VisualTreeHelper.GetChild(visual, i);
  //        MemberInfo[] sharedMembers = childVisual.GetType().GetMembers(
  //          BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

  //        foreach (MemberInfo member in sharedMembers)
  //        {
  //          DependencyProperty prop = null;
  //          if (member.MemberType == MemberTypes.Field)
  //            prop = ((FieldInfo)member).GetValue(childVisual) as DependencyProperty;
  //          else if (member.MemberType == MemberTypes.Property)
  //            prop = ((PropertyInfo)member).GetValue(childVisual, null) as DependencyProperty;

  //          if (prop != null)
  //          {
  //            //Binding bnd = (Binding)childVisual.GetValue(childVisual);// BindingOperations.GetBinding(childVisual, prop);
  //            //if (bnd != null && bnd.RelativeSource == null && bnd.Path != null && string.IsNullOrEmpty(bnd.ElementName))
  //            //  FoundBinding(bnd, (FrameworkElement)childVisual, prop);

  //          }
  //        }
  //        FindBindings(childVisual);
  //      }
  //    }
  //  }

  //  /// <summary>
  //  /// Called by
  //  /// <see cref="FindChildBindings"/> each
  //  /// time an object binding is found.
  //  /// </summary>
  //  /// <param name="bnd">The Binding object.</param>
  //  /// <param name="control">The control containing the binding.</param>
  //  /// <param name="prop">The data bound DependencyProperty.</param>
  //  protected virtual void FoundBinding(Binding bnd, FrameworkElement control, DependencyProperty prop)
  //  {
  //  }
  //}
}