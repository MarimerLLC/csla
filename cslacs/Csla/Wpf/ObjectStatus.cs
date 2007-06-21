#if !NET20
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
using Csla.Core;

namespace Csla.Wpf
{
  /// <summary>
  /// Container for other UI controls that exposes
  /// various status values from the CSLA .NET
  /// business object acting as DataContext.
  /// </summary>
  /// <remarks>
  /// This control provides access to the IsDirty,
  /// IsNew, IsDeleted, IsValid and IsSavable properties
  /// of a business object. The purpose behind this
  /// control is to expose those properties in a way
  /// that supports WFP data binding against those
  /// values.
  /// </remarks>
  public class ObjectStatus : DataDecoratorBase
  {
    #region Dependency Properties

    private static readonly DependencyProperty IsDeletedProperty =
      DependencyProperty.Register("IsDeleted", typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsDirtyProperty =
      DependencyProperty.Register("IsDirty", typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsNewProperty =
      DependencyProperty.Register("IsNew", typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsSavableProperty =
      DependencyProperty.Register("IsSavable", typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsValidProperty =
      DependencyProperty.Register("IsValid", typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);

    /// <summary>
    /// Exposes the IsDeleted property of the
    /// DataContext business object.
    /// </summary>
    public bool IsDeleted
    {
      get { return (bool)base.GetValue(IsDeletedProperty); }
      set 
      {
        bool old = IsDeleted;
        base.SetValue(IsDeletedProperty, value);
        OnPropertyChanged(
          new DependencyPropertyChangedEventArgs(IsDeletedProperty, old, value));
      }
    }

    /// <summary>
    /// Exposes the IsDirty property of the
    /// DataContext business object.
    /// </summary>
    public bool IsDirty
    {
      get { return (bool)base.GetValue(IsDirtyProperty); }
      set
      {
        bool old = IsDirty;
        base.SetValue(IsDirtyProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(IsDirtyProperty, old, value));
      }
    }

    /// <summary>
    /// Exposes the IsNew property of the
    /// DataContext business object.
    /// </summary>
    public bool IsNew
    {
      get { return (bool)base.GetValue(IsNewProperty); }
      set
      {
        bool old = IsNew;
        base.SetValue(IsNewProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(IsNewProperty, old, value));
      }
    }

    /// <summary>
    /// Exposes the IsSavable property of the
    /// DataContext business object.
    /// </summary>
    public bool IsSavable
    {
      get { return (bool)base.GetValue(IsSavableProperty); }
      set
      {
        bool old = IsSavable;
        base.SetValue(IsSavableProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(IsSavableProperty, old, value));
      }
    }

    /// <summary>
    /// Exposes the IsValid property of the
    /// DataContext business object.
    /// </summary>
    public bool IsValid
    {
      get { return (bool)base.GetValue(IsValidProperty); }
      set
      {
        bool old = IsValid;
        base.SetValue(IsValidProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(IsValidProperty, old, value));
      }
    }

    #endregion

    /// <summary>
    /// This method is called when the data
    /// object to which the control is bound
    /// has changed.
    /// </summary>
    protected override void DataObjectChanged()
    {
      Refresh();
    }

    /// <summary>
    /// This method is called when a property
    /// of the data object to which the 
    /// control is bound has changed.
    /// </summary>
    protected override void DataPropertyChanged(PropertyChangedEventArgs e)
    {
      Refresh();
    }

    /// <summary>
    /// This method is called if the data
    /// object is an IBindingList, and the 
    /// ListChanged event was raised by
    /// the data object.
    /// </summary>
    protected override void DataBindingListChanged(ListChangedEventArgs e)
    {
      Refresh();
    }

    /// <summary>
    /// This method is called if the data
    /// object is an INotifyCollectionChanged, 
    /// and the CollectionChanged event was 
    /// raised by the data object.
    /// </summary>
    protected override void DataObservableCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      Refresh();
    }

    /// <summary>
    /// Refreshes the control's property
    /// values to reflect the values of
    /// the underlying business object.
    /// </summary>
    public void Refresh()
    {
      IEditableBusinessObject source = DataObject as IEditableBusinessObject;
      if (source != null)
      {
        if (IsDeleted != source.IsDeleted)
          IsDeleted = source.IsDeleted;
        if (IsDirty != source.IsDirty)
          IsDirty = source.IsDirty;
        if (IsNew != source.IsNew)
          IsNew = source.IsNew;
        if (IsSavable != source.IsSavable)
          IsSavable = source.IsSavable;
        if (IsValid != source.IsValid)
          IsValid = source.IsValid;
      }
      else
      {
        IEditableCollection sourceList = DataObject as IEditableCollection;
        if (sourceList != null)
        {
          if (IsDirty != sourceList.IsDirty)
            IsDirty = sourceList.IsDirty;
          if (IsValid != sourceList.IsValid)
            IsValid = sourceList.IsValid;
          if (IsSavable != sourceList.IsSavable)
            IsSavable = sourceList.IsSavable;
          IsDeleted = false;
          IsNew = false;
        }
      }
    }
  }
}
#endif