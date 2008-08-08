using System;
using System.Windows;
using System.Windows.Data;
using System.Reflection;
using Csla.Security;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace Csla.Wpf
{
  [TemplateVisualState(Name = "AllowEdit", GroupName = "CanEdit")]
  [TemplateVisualState(Name = "DenyEdit", GroupName = "CanEdit")]
  [TemplateVisualState(Name = "AllowCreate", GroupName = "CanCreate")]
  [TemplateVisualState(Name = "DenyCreate", GroupName = "CanCreate")]
  [TemplateVisualState(Name = "AllowDelete", GroupName = "CanDelete")]
  [TemplateVisualState(Name = "DenyDelete", GroupName = "CanDelete")]
  [TemplateVisualState(Name = "AllowGet", GroupName = "CanGet")]
  [TemplateVisualState(Name = "DenyGet", GroupName = "CanGet")]
  public class ObjectStatus : Control
  {
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(object),
      typeof(ObjectStatus),
      new PropertyMetadata((o, e) => ((ObjectStatus)o).Source = e.NewValue));

    private object _source;

    public object Source
    {
      get { return _source; }
      set 
      {
        DetachSource(_source as INotifyPropertyChanged);
        _source = value;
        AttachSource(_source as INotifyPropertyChanged);
        OnSourceChanged();
      }
    }

    public ObjectStatus()
    {
      DefaultStyleKey = typeof(ObjectStatus);
    }

    private void AttachSource(INotifyPropertyChanged source)
    {
      if (source != null)
        source.PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);
    }

    private void DetachSource(INotifyPropertyChanged source)
    {
      if (source != null)
        source.PropertyChanged -= new PropertyChangedEventHandler(source_PropertyChanged);
    }

    private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnSourceChanged();
    }

    private void OnSourceChanged()
    {
      Type sourceType = null;
      if (Source != null)
        sourceType = Source.GetType();

      if(sourceType!=null)
      {
        if (Csla.Security.AuthorizationRules.CanCreateObject(sourceType))
          VisualStateManager.GoToState(this, "AllowCreate", true);
        else
          VisualStateManager.GoToState(this, "DenyCreate", true);

        if (Csla.Security.AuthorizationRules.CanDeleteObject(sourceType))
          VisualStateManager.GoToState(this, "AllowDelete", true);
        else
          VisualStateManager.GoToState(this, "DenyDelete", true);

        if (Csla.Security.AuthorizationRules.CanEditObject(sourceType))
          VisualStateManager.GoToState(this, "AllowEdit", true);
        else
          VisualStateManager.GoToState(this, "DenyEdit", true);

        if (Csla.Security.AuthorizationRules.CanGetObject(sourceType))
          VisualStateManager.GoToState(this, "AllowGet", true);
        else
          VisualStateManager.GoToState(this, "DenyGet", true);
      }
    }
  }
}