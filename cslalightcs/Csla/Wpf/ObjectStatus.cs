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
  public class ObjectStatus : ContentControl
  {
    public ObjectStatus()
    {
      DefaultStyleKey = typeof(ObjectStatus);
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
      DetachSource(oldContent as INotifyPropertyChanged);
      base.OnContentChanged(oldContent, newContent);
      AttachSource(newContent as INotifyPropertyChanged);
      OnSourceChanged();
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
      if (Content != null)
        sourceType = Content.GetType();
      if (DataContext != null)
        sourceType = DataContext.GetType();

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