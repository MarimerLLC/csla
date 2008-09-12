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
using System.Collections.Generic;
using System.Reflection;

namespace Csla.Data
{

  public class DataServiceContextManager<C> where C : System.Data.Services.Client.DataServiceContext
  {

    private static object _lock = new object();
    private C _context;

    public static DataServiceContextManager<C> GetManager(Uri path)
    {

      lock (_lock)
      {
        DataServiceContextManager<C> mgr = null;
        if (ApplicationContext.LocalContext.ContainsKey("__ctx:" + path.ToString()))
        {
          mgr = (DataServiceContextManager<C>)(ApplicationContext.LocalContext["__ctx:" + path.ToString()]);
        }
        else
        {
          mgr = new DataServiceContextManager<C>(path);
          ApplicationContext.LocalContext["__ctx:" + path.ToString()] = mgr;
        }
        return mgr;
      }
    }

    private DataServiceContextManager(Uri path)
    {
      _context = (C)(Activator.CreateInstance(typeof(C), path));
    }

    public C DataServiceContext
    {
      get
      {
        return _context;
      }
    }

    public List<T> GetEntities<T>()
    {
      List<T> returnValue = new List<T>();
      foreach (var oneEntityDescriptor in _context.Entities)
      {
        if (oneEntityDescriptor.Entity is T)
        {
          returnValue.Add((T)oneEntityDescriptor.Entity);
        }
      }
      return returnValue;
    }

    public T GetEntity<T>(string keyPropertyName, object keyPropertyValue)
    {
      T returnValue = default(T);
      PropertyInfo prop = Csla.Reflection.MethodCaller.GetProperty(typeof(T), keyPropertyName);

      foreach (T oneEntity in GetEntities<T>())
      {
        if (keyPropertyValue.Equals(Csla.Reflection.MethodCaller.GetPropertyValue(oneEntity, prop)))
        {
          returnValue = oneEntity;
          break;
        }
      }
      return returnValue;
    }
  }
}
