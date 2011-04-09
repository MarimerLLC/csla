using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.ComponentModel;

namespace MonoAndroidApplication1
{
  public class Binding : IDisposable
  {
    public View Target { get; private set; }
    public object Source { get; private set; }
    public System.Reflection.PropertyInfo TargetProperty { get; private set; }
    public System.Reflection.PropertyInfo SourceProperty { get; private set; }
    public Func<object, object> Convert { get; private set; }
    public Func<object, object> ConvertBack { get; private set; }

    public Binding(View target, string targetProperty, object source, string sourceProperty)
      : this(target, targetProperty, source, sourceProperty, null, null)
    { }

    public Binding(View target, string targetProperty, object source, string sourceProperty, Func<object, object> convert)
      : this(target, targetProperty, source, sourceProperty, convert, null)
    { }

    public Binding(View target, string targetProperty, object source, string sourceProperty, Func<object, object> convert, Func<object, object> convertBack)
    {
      Target = target;
      Source = source;
      TargetProperty = Target.GetType().GetProperty(targetProperty);
      SourceProperty = Source.GetType().GetProperty(sourceProperty);
      Convert = convert;
      ConvertBack = convertBack;

      Target.FocusChange += Target_FocusChange;

      var inpc = Source as INotifyPropertyChanged;
      if (inpc != null)
        inpc.PropertyChanged += Source_PropertyChanged;

      UpdateTarget();
    }

    void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      UpdateTarget();
    }

    void Target_FocusChange(object sender, View.FocusChangeEventArgs e)
    {
      if (!e.HasFocus)
        UpdateSource();
    }

    public void UpdateSource()
    {
      var value = TargetProperty.GetValue(Target, null);
      if (ConvertBack != null)
        value = ConvertBack(value);
      var converted = Csla.Utilities.CoerceValue(
        SourceProperty.PropertyType, TargetProperty.PropertyType, null, value);
      SourceProperty.SetValue(Source, 
        converted, 
        null);
    }

    public void UpdateTarget()
    {
      var value = SourceProperty.GetValue(Source, null);
      if (Convert != null)
        value = Convert(value);
      var converted = Csla.Utilities.CoerceValue(
        TargetProperty.PropertyType, SourceProperty.PropertyType, null, value);
      TargetProperty.SetValue(Target,
        converted,
        null);
    }

    public void Dispose()
    {
      Target.FocusChange -= Target_FocusChange;

      var inpc = Source as INotifyPropertyChanged;
      if (inpc != null)
        inpc.PropertyChanged -= Source_PropertyChanged;
      Target = null;
      Source = null;
      TargetProperty = null;
      SourceProperty = null;
    }
  }
}