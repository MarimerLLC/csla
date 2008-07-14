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

    private TestResult _status;
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

    public TestResult Status
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
      _status = TestResult.Evaluating;
    }

    #endregion

    public void RunTest()
    {
      IsRunning = true;
      ExpectedExceptionAttribute expectedException = null;
      PropertyInfo contextProperty = _method.DeclaringType.GetProperty("Context");

      bool isAsync = (contextProperty != null && typeof(UnitTestContext).IsAssignableFrom(contextProperty.PropertyType));
      bool expectsException = _method.IsDefined(typeof(ExpectedExceptionAttribute), true);
      
      if(expectsException)
      {
        object[] attributes = _method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), true);
        if(attributes.Length > 1)
          throw new ArgumentException("Can only have one ExpectedExceptionAttribute per method");

        expectedException = attributes[0] as ExpectedExceptionAttribute;
      }

      object instance = InitializeInstance();
      if(isAsync)
      {
        UnitTestContext context = (UnitTestContext)contextProperty.GetValue(instance, null);
        context.Completed += (o, e) =>
        {
          if(expectsException)
          {
            if (e.Status == TestResult.Fail &&
              e.Error != null &&
              expectedException.Type.IsAssignableFrom(e.Error.GetType()))
            {
              Status = TestResult.Success;
            }
            else Status = TestResult.Fail;
          }
          else
            Status = e.Status;

          if (Status == TestResult.Fail && e.Error != null)
          {
            Message = string.Format("{0}: {1}", 
              e.Error.Innermost().GetType().FullName, 
              e.Error.Innermost().Message);
          }            
        };

        try
        {
          _method.Invoke(instance, null);
        }
        catch(Exception ex)
        {
          Status = TestResult.Fail;
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
            Status = TestResult.Fail;
          }
          else
          {
            Status = TestResult.Success;
          }
        }
        catch(Exception ex)
        {
          if (expectsException && expectedException.Type.IsAssignableFrom(ex.InnerException.GetType()))
          {
            Status = TestResult.Success;
          }
          else
          {
            Status = TestResult.Fail;
            Message = string.Format("{0}: {1}", ex.Innermost().GetType().FullName, ex.Innermost().Message);
          }
        }
      }
    }

    private object InitializeInstance()
    {
      object instance = Activator.CreateInstance(_method.DeclaringType);
      foreach (MethodInfo m in _method.DeclaringType.GetMethods())
      {
        if (m.IsDefined(typeof(TestSetup), true))
        {
          m.Invoke(instance, null);
          break;
        }
      }
      return instance;
    }
  }
}
