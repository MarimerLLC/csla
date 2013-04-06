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

namespace Csla.Data
{
  public class ServiceClientManager<C, T>
    where C : System.ServiceModel.ClientBase<T>
    where T : class
  {
    private static object _lock = new object();
    private C _client;
    private string _name = string.Empty;

    public static ServiceClientManager<C,T > GetManager(string name)
    {

      lock (_lock)
      {
        ServiceClientManager<C, T> mgr = null;
        if (ApplicationContext.LocalContext.ContainsKey(name))
        {
          mgr = (ServiceClientManager<C, T>)(ApplicationContext.LocalContext[name]);
        }
        else
        {
          mgr = new ServiceClientManager<C, T>(name);
          ApplicationContext.LocalContext[name] = mgr;
        }
        return mgr;
      }
    }


    private ServiceClientManager(string name)
    {
      _client = (C)(Activator.CreateInstance(typeof(C)));
    }

    public C Client
    {
      get
      {
        return _client;
      }
    }
  }
}
