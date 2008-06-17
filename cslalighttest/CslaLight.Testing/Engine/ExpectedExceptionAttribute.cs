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

namespace cslalighttest.Engine
{
  [AttributeUsage(AttributeTargets.Method)]
  public class ExpectedExceptionAttribute : Attribute
  {
    private Type _type;
    public Type Type { get { return _type; } }

    public ExpectedExceptionAttribute(Type exceptionType)
    {
      if (!typeof(Exception).IsAssignableFrom(exceptionType))
        throw new ArgumentException(
          string.Format("'{0}' does not inherit from Exception", exceptionType), 
          "exceptionType");

      _type = exceptionType;
    }
  }
}
