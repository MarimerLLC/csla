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
  public class ObjectStatusPanel : DataPanelBase, INotifyPropertyChanged
  {
    #region INotifyPropertyChanged

    /// <summary>
    /// Event indicating that an object property has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise the PropertyChanged event.
    /// </summary>
    /// <param name="name">Name of the property that has changed.</param>
    protected void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }

    #endregion

    #region Dependency Properties

    private static readonly DependencyProperty IsDeletedProperty =
      DependencyProperty.Register("IsDeleted", typeof(bool), typeof(ObjectStatusPanel), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsDirtyProperty =
      DependencyProperty.Register("IsDirty", typeof(bool), typeof(ObjectStatusPanel), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsNewProperty =
      DependencyProperty.Register("IsNew", typeof(bool), typeof(ObjectStatusPanel), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsSavableProperty =
      DependencyProperty.Register("IsSavable", typeof(bool), typeof(ObjectStatusPanel), new FrameworkPropertyMetadata(false), null);
    private static readonly DependencyProperty IsValidProperty =
      DependencyProperty.Register("IsValid", typeof(bool), typeof(ObjectStatusPanel), new FrameworkPropertyMetadata(false), null);

    /// <summary>
    /// Exposes the IsDeleted property of the
    /// DataContext business object.
    /// </summary>
    public bool IsDeleted
    {
      get { return (bool)base.GetValue(IsDeletedProperty); }
      set { base.SetValue(IsDeletedProperty, value); }
    }

    /// <summary>
    /// Exposes the IsDirty property of the
    /// DataContext business object.
    /// </summary>
    public bool IsDirty
    {
      get { return (bool)base.GetValue(IsDirtyProperty); }
      set { base.SetValue(IsDirtyProperty, value); }
    }

    /// <summary>
    /// Exposes the IsNew property of the
    /// DataContext business object.
    /// </summary>
    public bool IsNew
    {
      get { return (bool)base.GetValue(IsNewProperty); }
      set { base.SetValue(IsNewProperty, value); }
    }

    /// <summary>
    /// Exposes the IsSavable property of the
    /// DataContext business object.
    /// </summary>
    public bool IsSavable
    {
      get { return (bool)base.GetValue(IsSavableProperty); }
      set { base.SetValue(IsSavableProperty, value); }
    }

    /// <summary>
    /// Exposes the IsValid property of the
    /// DataContext business object.
    /// </summary>
    public bool IsValid
    {
      get { return (bool)base.GetValue(IsValidProperty); }
      set { base.SetValue(IsValidProperty, value); }
    }

    #endregion

    /// <summary>
    /// This method is called when the data
    /// object to which the control is bound
    /// has changed.
    /// </summary>
    protected override void DataObjectChanged()
    {
      StatusScan();
    }

    /// <summary>
    /// This method is called when a property
    /// of the data object to which the 
    /// control is bound has changed.
    /// </summary>
    protected override void DataPropertyChanged(PropertyChangedEventArgs e)
    {
      StatusScan();
    }

    private void StatusScan()
    {
      IEditableBusinessObject source = DataObject as IEditableBusinessObject;
      if (source != null)
      {
        if (IsDeleted != source.IsDeleted)
        {
          IsDeleted = source.IsDeleted;
          OnPropertyChanged("IsDeleted");
        }
        if (IsDirty != source.IsDirty)
        {
          IsDirty = source.IsDirty;
          OnPropertyChanged("IsDirty");
        }
        if (IsNew != source.IsNew)
        {
          IsNew = source.IsNew;
          OnPropertyChanged("IsNew");
        }
        if (IsSavable != source.IsSavable)
        {
          IsSavable = source.IsSavable;
          OnPropertyChanged("IsSavable");
        }
        if (IsValid != source.IsValid)
        {
          IsValid = source.IsValid;
          OnPropertyChanged("IsValid");
        }
      }
    }
  }
}
