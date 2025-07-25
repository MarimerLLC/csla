#if !XAMARIN && !WINDOWS_UWP && !MAUI
//-----------------------------------------------------------------------
// <copyright file="DataDecoratorBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class for creating WPF panel</summary>
//-----------------------------------------------------------------------
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Csla.Xaml
{
  /// <summary>
  /// Base class for creating WPF panel
  /// controls that react when the DataContext,
  /// data object and data property values
  /// are changed.
  /// </summary>
  public class DataDecoratorBase : Decorator
  {
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
    protected object? DataObject { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public DataDecoratorBase()
    {
      DataContextChanged += Panel_DataContextChanged;
      Loaded += Panel_Loaded;
    }

    private void Panel_Loaded(object? sender, RoutedEventArgs e)
    {
      UpdateDataObject(null, DataObject);
    }

    /// <summary>
    /// Handle case where the DataContext for the
    /// control has changed.
    /// </summary>
    private void Panel_DataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
      UpdateDataObject(e.OldValue, e.NewValue);
    }

    private object? GetDataObject(object? dataContext)
    {
      object? result = dataContext;
      if (dataContext is DataSourceProvider provider)
      {
        result = provider.Data;
      }
      else if (dataContext is ICollectionView icv)
      {
        result = icv.CurrentItem;
      }
      return result;
    }

    /// <summary>
    /// Handle case where the Data property of the
    /// DataContext (a DataSourceProvider) has changed.
    /// </summary>
    private void DataProvider_DataChanged(object? sender, EventArgs e)
    {
      if (sender is null)
        throw new ArgumentNullException(nameof(sender));
      if (sender is not DataSourceProvider dsp)
        throw new ArgumentException($"{sender.GetType()} != {nameof(DataSourceProvider)}", nameof(sender));

      UpdateDataObject(DataObject, dsp.Data);
    }

    private void UpdateDataObject(object? oldObject, object? newObject)
    {
      if (!ReferenceEquals(oldObject, newObject))
      {
        if (oldObject != null)
          UnHookDataContextEvents(oldObject);

        // store a ref to the data object
        DataObject = GetDataObject(newObject);

        if (newObject != null)
          HookDataContextEvents(newObject);

        DataObjectChanged();
      }
    }

    #region Hook/unhook events

    private void UnHookDataContextEvents(object oldValue)
    {
      // unhook any old event handling
      object oldContext;

      if (oldValue is not DataSourceProvider provider)
      {
        oldContext = oldValue;
      }
      else
      {
        provider.DataChanged -= DataProvider_DataChanged;
        oldContext = provider.Data;
      }
      UnHookChildChanged(oldContext as Core.INotifyChildChanged);
      UnHookPropertyChanged(oldContext as INotifyPropertyChanged);
      if (oldContext is INotifyCollectionChanged observable)
        UnHookObservableListChanged(observable);
      else
        UnHookBindingListChanged(oldContext as IBindingList);
    }

    private void HookDataContextEvents(object newValue)
    {
      // hook any new event
      object newContext;

      if (newValue is not DataSourceProvider provider)
      {
        newContext = newValue;
      }
      else
      {
        provider.DataChanged += DataProvider_DataChanged;
        newContext = provider.Data;
      }
      HookChildChanged(newContext as Core.INotifyChildChanged);
      HookPropertyChanged(newContext as INotifyPropertyChanged);
      if (newContext is INotifyCollectionChanged observable)
        HookObservableListChanged(observable);
      else
        HookBindingListChanged(newContext as IBindingList);
    }

    private void UnHookPropertyChanged(INotifyPropertyChanged? oldContext)
    {
      if (oldContext != null)
        oldContext.PropertyChanged -= DataObject_PropertyChanged;
    }

    private void HookPropertyChanged(INotifyPropertyChanged? newContext)
    {
      if (newContext != null)
        newContext.PropertyChanged += DataObject_PropertyChanged;
    }

    private void UnHookChildChanged(Core.INotifyChildChanged? oldContext)
    {
      if (oldContext != null)
        oldContext.ChildChanged -= DataObject_ChildChanged;
    }

    private void HookChildChanged(Core.INotifyChildChanged? newContext)
    {
      if (newContext != null)
        newContext.ChildChanged += DataObject_ChildChanged;
    }

    private void UnHookBindingListChanged(IBindingList? oldContext)
    {
      if (oldContext != null)
        oldContext.ListChanged -= DataObject_ListChanged;
    }

    private void HookBindingListChanged(IBindingList? newContext)
    {
      if (newContext != null)
        newContext.ListChanged += DataObject_ListChanged;
    }

    private void UnHookObservableListChanged(INotifyCollectionChanged? oldContext)
    {
      if (oldContext != null)
        oldContext.CollectionChanged -= DataObject_CollectionChanged;
    }

    private void HookObservableListChanged(INotifyCollectionChanged? newContext)
    {
      if (newContext != null)
        newContext.CollectionChanged += DataObject_CollectionChanged;
    }

    #endregion

    #region Handle events

    private void DataObject_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      DataPropertyChanged(e);
    }

    private void DataObject_ChildChanged(object? sender, Core.ChildChangedEventArgs e)
    {
      DataPropertyChanged(e.PropertyChangedArgs);
    }

    private void DataObject_ListChanged(object? sender, ListChangedEventArgs e)
    {
      DataBindingListChanged(e);
    }

    private void DataObject_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      DataObservableCollectionChanged(e);
    }

    #endregion

    #region Virtual methods

    /// <summary>
    /// This method is called when a property
    /// of the data object to which the 
    /// control is bound has changed.
    /// </summary>
    protected virtual void DataPropertyChanged(PropertyChangedEventArgs? e)
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

    #endregion

    #region FindingBindings

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
        MemberInfo[] sharedMembers = childVisual.GetType().GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        foreach (MemberInfo member in sharedMembers)
        {
          DependencyProperty? prop = null;
          if (member.MemberType == MemberTypes.Field)
            prop = ((FieldInfo)member).GetValue(childVisual) as DependencyProperty;
          else if (member.MemberType == MemberTypes.Property)
            prop = ((System.Reflection.PropertyInfo)member).GetValue(childVisual, null) as DependencyProperty;

          if (prop != null)
          {
            Binding? bnd = BindingOperations.GetBinding(childVisual, prop);
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

    #endregion
  }
}
#endif