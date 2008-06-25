using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace cslalighttest.Engine
{
  public class MethodTester : ObservableObject
  {
    #region Member fields and properties

    private MethodTesterStatus _status;
    private MethodInfo _method;
    private string _message;
    private bool _isRunning;

    public bool IsRunning
    {
      get { return _isRunning; }
      set
      {
        _isRunning = value;
        OnPropertyChanged("IsRunning");
        OnPropertyChanged("IsNotRunning");
      }
    }
    public bool IsNotRunning
    {
      get { return !_isRunning; }
    }

    public MethodTesterStatus Status
    {
      get { return _status; }
      protected set
      {
        _status = value;
        OnPropertyChanged("Status");

        IsRunning = false;
      }
    }

    public string Message
    {
      get { return _message; }
      private set
      {
        _message = value;
        OnPropertyChanged("Message");
      }
    }

    public string Name { get { return _method.Name; } }

    public MethodInfo Method
    {
      get { return _method; }
    }

    #endregion

    #region Constructors

    public MethodTester(MethodInfo method)
    {
      if (!method.IsDefined(typeof(TestMethodAttribute), true))
        throw new ArgumentException("Method must have TestMethod attribute applied");
      if (!method.IsPublic)
        throw new ArgumentException("Method must be public");

      _method = method;
      _status = MethodTesterStatus.Evaluating;
    }

    #endregion

    public void RunTest(object instance)
    {
      IsRunning = true;
      ParameterInfo[] parameters = _method.GetParameters();
      ExpectedExceptionAttribute expectedException = null;

      bool isAsync = (parameters.Length > 0 && typeof(AsyncTestContext).IsAssignableFrom(parameters[0].ParameterType));
      bool expectsException = _method.IsDefined(typeof(ExpectedExceptionAttribute), true);
      
      if(expectsException)
      {
        object[] attributes = _method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), true);
        if(attributes.Length > 1)
          throw new ArgumentException("Can only have one ExpectedExceptionAttribute per method");

        expectedException = attributes[0] as ExpectedExceptionAttribute;
      }

      foreach (MethodInfo m in _method.DeclaringType.GetMethods())
      {
        if (m.IsDefined(typeof(TestSetup), true))
        {
          m.Invoke(instance, null);
          break;
        }
      }

      if(isAsync)
      {
        AsyncTestContext context = new AsyncTestContext();
        context.Complete += (o, e) =>
          {
            if(expectsException)
            {
              if(e.Status == MethodTesterStatus.Fail && expectedException.Type.IsAssignableFrom( e.Error.GetType()))
                Status = MethodTesterStatus.Success;
              else
                Status = MethodTesterStatus.Fail;
            }
            else
              Status = e.Status;

            if (e.Error != null)
            {
              Message = string.Format("{0}: {1}", 
                e.Error.Innermost().GetType().FullName, 
                e.Error.Innermost().Message);
            }            
          };

        try
        {
          _method.Invoke(instance, new object[]{ context });
        }
        catch(Exception ex)
        {
          Status = MethodTesterStatus.Fail;
          Message = string.Format("{0}: {1}", ex.Innermost().GetType().FullName, ex.Innermost().Message);
        }
      }
      else
      {
        try
        {
          _method.Invoke(instance, null);
          if (expectsException)
          {
            Status = MethodTesterStatus.Fail;
          }
          else
          {
            Status = MethodTesterStatus.Success;
          }
        }
        catch(Exception ex)
        {
          if (expectsException && expectedException.Type.IsAssignableFrom(ex.InnerException.GetType()))
          {
            Status = MethodTesterStatus.Success;
          }
          else
          {
            Status = MethodTesterStatus.Fail;
            Message = string.Format("{0}: {1}", ex.Innermost().GetType().FullName, ex.Innermost().Message);
          }
        }
      }
    }
  }
}
