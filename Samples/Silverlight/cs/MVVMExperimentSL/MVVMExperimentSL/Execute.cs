using System;
using System.Windows;
using System.Windows.Interactivity;

namespace MVVMexperiment
{
  public class Execute : TriggerAction<FrameworkElement>
  {
    public static readonly DependencyProperty MethodNameProperty =
      DependencyProperty.Register("MethodName", typeof(string),
      typeof(Execute), new PropertyMetadata(null));
    public string MethodName
    {
      get
      {
        return (string)GetValue(MethodNameProperty);
      }
      set
      {
        SetValue(MethodNameProperty, value);
      }
    }

    public static readonly DependencyProperty MethodParameterProperty =
      DependencyProperty.Register("MethodParameter", typeof(object),
      typeof(Execute), new PropertyMetadata(null));
    public object MethodParameter
    {
      get
      {
        return (object)GetValue(MethodParameterProperty);
      }
      set
      {
        SetValue(MethodParameterProperty, value);
      }
    }

    protected override void Invoke(object parameter)
    {
      var methodName = this.MethodName;
      if (!string.IsNullOrEmpty(methodName))
      {
        var element = this.AssociatedObject;
        var target = element.DataContext;
        if (target != null)
        {
          var obj = new Csla.Reflection.LateBoundObject(target);
          obj.CallMethod(methodName, this, new ExecuteEventArgs 
            { 
              TriggerSource = element,
              MethodParamter = this.MethodParameter,
              TriggerParameter = parameter
            });
        }
      }
    }

    protected override void OnAttached()
    {
      base.OnAttached();
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
    }
  }

  public class ExecuteEventArgs : EventArgs
  {
    public FrameworkElement TriggerSource { get; set; }
    public object MethodParamter { get; set; }
    public object TriggerParameter { get; set; }
  }
}
