using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Csla.Core.LoadManager
{
  internal class AsyncLoader
  {
    IPropertyInfo _property;
    Action<IPropertyInfo, object> _loadProperty;
    Action<string> _propertyChanged;
    Delegate _factory;
    object[] _parameters;

    public IPropertyInfo Property { get { return _property; } }

    public AsyncLoader(
      IPropertyInfo property, 
      Delegate factory, 
      Action<IPropertyInfo, object> loadProperty, 
      Action<string> propertyChanged,
      params object[] parameters)
    {
      _property = property;
      _loadProperty = loadProperty;
      _propertyChanged = propertyChanged;
      _factory = factory;
      _parameters = parameters;
    }

    public event EventHandler<ErrorEventArgs> Complete;
    protected void OnComplete(Exception error)
    {
      if (Complete != null)
        Complete(this, new ErrorEventArgs(this, error));
    }

    internal void Load(Delegate handler)
    {
      List<object> parameters = new List<object>(_parameters);
      parameters.Insert(0, handler);

      _factory.DynamicInvoke(parameters.ToArray());
    }

    public void LoadComplete<R>(object sender, DataPortalResult<R> result)
    {
      R obj = result.Object;

      _loadProperty(_property, obj);
      _propertyChanged(_property.Name);
      OnComplete(result.Error);
    }
  }
}
