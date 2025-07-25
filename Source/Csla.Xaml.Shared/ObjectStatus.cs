#if !XAMARIN && !WINDOWS_UWP && !MAUI
//-----------------------------------------------------------------------
// <copyright file="ObjectStatus.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Container for other UI controls that exposes</summary>
//-----------------------------------------------------------------------
using System.ComponentModel;
using System.Windows;
using Csla.Core;

namespace Csla.Xaml
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
    #region Per-Type Dependency Properties

    private static readonly DependencyProperty CanCreateProperty =
      DependencyProperty.Register(nameof(CanCreateObject), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty CanGetProperty =
      DependencyProperty.Register(nameof(CanGetObject), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty CanEditProperty =
      DependencyProperty.Register(nameof(CanEditObject), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty CanDeleteProperty =
      DependencyProperty.Register(nameof(CanDeleteObject), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);

    /// <summary>
    /// Exposes the CanCreateObject property of the
    /// DataContext business object.
    /// </summary>
    public bool CanCreateObject
    {
      get => (bool)GetValue(CanCreateProperty);
      protected set
      {
        bool old = CanCreateObject;
        SetValue(CanCreateProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(CanCreateProperty, old, value));
      }
    }

    /// <summary>
    /// Exposes the CanGetObject property of the
    /// DataContext business object.
    /// </summary>
    public bool CanGetObject
    {
      get => (bool)GetValue(CanGetProperty);
      protected set
      {
        bool old = CanGetObject;
        SetValue(CanGetProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(CanGetProperty, old, value));
      }
    }

    /// <summary>
    /// Exposes the CanEditObject property of the
    /// DataContext business object.
    /// </summary>
    public bool CanEditObject
    {
      get => (bool)GetValue(CanEditProperty);
      protected set
      {
        bool old = CanEditObject;
        SetValue(CanEditProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(CanEditProperty, old, value));
      }
    }

    /// <summary>
    /// Exposes the CanDeleteObject property of the
    /// DataContext business object.
    /// </summary>
    public bool CanDeleteObject
    {
      get => (bool)GetValue(CanDeleteProperty);
      protected set
      {
        bool old = CanDeleteObject;
        SetValue(CanDeleteProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(CanDeleteProperty, old, value));
      }
    }

    #endregion

    #region Per-Instance Dependency Properties

    private static readonly DependencyProperty IsDeletedProperty =
      DependencyProperty.Register(nameof(IsDeleted), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsDirtyProperty =
      DependencyProperty.Register(nameof(IsDirty), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsNewProperty =
      DependencyProperty.Register(nameof(IsNew), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsSavableProperty =
      DependencyProperty.Register(nameof(IsSavable), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsValidProperty =
      DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(ObjectStatus), new FrameworkPropertyMetadata(false), null);

    /// <summary>
    /// Exposes the IsDeleted property of the
    /// DataContext business object.
    /// </summary>
    public bool IsDeleted
    {
      get => (bool)GetValue(IsDeletedProperty);
      protected set
      {
        bool old = IsDeleted;
        SetValue(IsDeletedProperty, value);
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
      get => (bool)GetValue(IsDirtyProperty);
      protected set
      {
        bool old = IsDirty;
        SetValue(IsDirtyProperty, value);
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
      get => (bool)GetValue(IsNewProperty);
      protected set
      {
        bool old = IsNew;
        SetValue(IsNewProperty, value);
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
      get => (bool)GetValue(IsSavableProperty);
      protected set
      {
        bool old = IsSavable;
        SetValue(IsSavableProperty, value);
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
      get => (bool)GetValue(IsValidProperty);
      protected set
      {
        bool old = IsValid;
        SetValue(IsValidProperty, value);
        if (old != value)
          OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(IsValidProperty, old, value));
      }
    }

    #endregion

    #region Base Overrides 

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
    protected override void DataPropertyChanged(PropertyChangedEventArgs? e)
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
      // per-type rules
      if (DataObject != null)
      {
        var newValue = Rules.BusinessRules.HasPermission(ApplicationContextManager.GetApplicationContext(), Rules.AuthorizationActions.CreateObject, DataObject);
        if (CanCreateObject != newValue)
          CanCreateObject = newValue;
        newValue = Rules.BusinessRules.HasPermission(ApplicationContextManager.GetApplicationContext(), Rules.AuthorizationActions.GetObject, DataObject);
        if (CanGetObject != newValue)
          CanGetObject = newValue;
        newValue = Rules.BusinessRules.HasPermission(ApplicationContextManager.GetApplicationContext(), Rules.AuthorizationActions.EditObject, DataObject);
        if (CanEditObject != newValue)
          CanEditObject = newValue;
        newValue = Rules.BusinessRules.HasPermission(ApplicationContextManager.GetApplicationContext(), Rules.AuthorizationActions.DeleteObject, DataObject);
        if (CanDeleteObject != newValue)
          CanDeleteObject = newValue;
      }
      else
      {
        CanCreateObject = false;
        CanGetObject = false;
        CanEditObject = false;
        CanDeleteObject = false;
      }

      if (DataObject is IEditableBusinessObject source)
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
        if (DataObject is IEditableCollection sourceList)
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

    #endregion
  }
}
#endif