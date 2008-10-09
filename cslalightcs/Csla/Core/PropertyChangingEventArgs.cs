using System;

namespace Csla.Core
{
  public class PropertyChangingEventArgs: System.EventArgs
  {
    private string _propertyName = string.Empty;
    public PropertyChangingEventArgs(string propertyName)
    {
      _propertyName = propertyName;
    }

    public virtual string PropertyName 
    {
      get { return _propertyName; }
    }
  }
}
