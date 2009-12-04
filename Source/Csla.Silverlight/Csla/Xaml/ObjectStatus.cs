using System;
using System.Windows;
using System.Windows.Data;
using System.Reflection;
using Csla.Security;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Windows.Input;
using Csla.Core;

namespace Csla.Xaml
{
  /// <summary>
  /// Object status control.
  /// </summary>
  [TemplateVisualState(Name = "Busy", GroupName = "IsBusy")]
  [TemplateVisualState(Name = "Idle", GroupName = "IsBusy")]
  [TemplateVisualState(Name = "AllowEdit", GroupName = "CanEdit")]
  [TemplateVisualState(Name = "DenyEdit", GroupName = "CanEdit")]
  [TemplateVisualState(Name = "AllowCreate", GroupName = "CanCreate")]
  [TemplateVisualState(Name = "DenyCreate", GroupName = "CanCreate")]
  [TemplateVisualState(Name = "AllowDelete", GroupName = "CanDelete")]
  [TemplateVisualState(Name = "DenyDelete", GroupName = "CanDelete")]
  [TemplateVisualState(Name = "AllowGet", GroupName = "CanGet")]
  [TemplateVisualState(Name = "DenyGet", GroupName = "CanGet")]
  [TemplateVisualState(Name = "AllowSave", GroupName = "CanSave")]
  [TemplateVisualState(Name = "DenySave", GroupName = "CanSave")]
  public class ObjectStatus : Control
  {
    /// <summary>
    /// Source property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(object),
      typeof(ObjectStatus),
      new PropertyMetadata((o, e) => ((ObjectStatus)o).Source = e.NewValue));

    /// <summary>
    /// IsBusy property.
    /// </summary>
    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
      "IsBusy",
      typeof(bool),
      typeof(ObjectStatus),
      new PropertyMetadata((o, e) => ((ObjectStatus)o).IsBusy = (bool)e.NewValue));

    /// <summary>
    /// Gets the IsBusy value.
    /// </summary>
    public bool IsBusy
    {
      get { return (bool)GetValue(IsBusyProperty); }
      protected set { SetValue(IsBusyProperty, value); }
    }

    private object _source;

    /// <summary>
    /// Gets or sets the source object.
    /// </summary>
    public object Source
    {
      get { return _source; }
      set
      {
        DetachSource(_source);
        _source = value;
        AttachSource(_source);
        OnSourceChanged();
      }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public ObjectStatus()
    {
      DefaultStyleKey = typeof(ObjectStatus);
      Loaded += new RoutedEventHandler(ObjectStatus_Loaded);
    }

    void ObjectStatus_Loaded(object sender, RoutedEventArgs e)
    {
      GoToState(true);
    }

    private void AttachSource(object source)
    {
      INotifyPropertyChanged npc = source as INotifyPropertyChanged;
      if (npc != null)
        npc.PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);

      INotifyBusy npb = source as INotifyBusy;
      if (npb != null)
        npb.BusyChanged += new BusyChangedEventHandler(source_PropertyBusy);
    }

    private void DetachSource(object source)
    {
      INotifyPropertyChanged npc = source as INotifyPropertyChanged;
      if (npc != null)
        npc.PropertyChanged -= new PropertyChangedEventHandler(source_PropertyChanged);

      INotifyBusy npb = source as INotifyBusy;
      if (npb != null)
        npb.BusyChanged -= new BusyChangedEventHandler(source_PropertyBusy);
    }

    void source_PropertyBusy(object sender, BusyChangedEventArgs e)
    {
      INotifyBusy busy = (INotifyBusy)sender;
      IsBusy = busy.IsBusy;
      GoToState(true);
    }

    private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnSourceChanged();
    }

    /// <summary>
    /// Invoked when the source changes.
    /// </summary>
    protected virtual void OnSourceChanged()
    {
      GoToState(true);
    }

    private void GoToState(bool useTransitions)
    {
      Type sourceType = null;
      if (Source != null)
        sourceType = Source.GetType();

      if (sourceType != null)
      {
        if (Csla.Security.AuthorizationRules.CanCreateObject(sourceType))
          VisualStateManager.GoToState(this, "AllowCreate", useTransitions);
        else
          VisualStateManager.GoToState(this, "DenyCreate", useTransitions);

        if (Csla.Security.AuthorizationRules.CanDeleteObject(sourceType))
          VisualStateManager.GoToState(this, "AllowDelete", useTransitions);
        else
          VisualStateManager.GoToState(this, "DenyDelete", useTransitions);

        if (Csla.Security.AuthorizationRules.CanEditObject(sourceType))
          VisualStateManager.GoToState(this, "AllowEdit", useTransitions);
        else
          VisualStateManager.GoToState(this, "DenyEdit", useTransitions);

        if (Csla.Security.AuthorizationRules.CanGetObject(sourceType))
          VisualStateManager.GoToState(this, "AllowGet", useTransitions);
        else
          VisualStateManager.GoToState(this, "DenyGet", useTransitions);

        ITrackStatus trackable = Source as ITrackStatus;
        if (trackable != null)
        {
          if (trackable.IsSavable)
            VisualStateManager.GoToState(this, "AllowSave", useTransitions);
          else
            VisualStateManager.GoToState(this, "DenySave", useTransitions);
        }
      }

      if(IsBusy)
        VisualStateManager.GoToState(this, "Busy", useTransitions);
      else
        VisualStateManager.GoToState(this, "Idle", useTransitions);
    }
  }
}