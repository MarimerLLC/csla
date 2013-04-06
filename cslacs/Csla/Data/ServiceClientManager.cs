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
  /// <summary>
  /// Provides an automated way to reuse 
  /// a service client proxy objects within 
  /// the context of a single data portal operation.
  /// </summary>
  /// <typeparam name="C">
  /// Type of ClientBase object to use.
  /// </typeparam>
  /// <typeparam name="T">
  /// Channel type for the ClientBase object.
  /// </typeparam>
  public class ServiceClientManager<C, T>
    where C : System.ServiceModel.ClientBase<T>
    where T : class
  {
    private static object _lock = new object();
    private C _client;
    private string _name = string.Empty;

    /// <summary>
    /// Gets the client proxy object for the
    /// specified name.
    /// </summary>
    /// <param name="name">Unique name for the proxy object.</param>
    /// <returns></returns>
    public static ServiceClientManager<C,T > GetManager(string name)
    {

      lock (_lock)
      {
        ServiceClientManager<C, T> mgr = null;
        if (ApplicationContext.LocalContext.Contains(name))
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

    /// <summary>
    /// Gets a reference to the current client proxy object.
    /// </summary>
    public C Client
    {
      get
      {
        return _client;
      }
    }
  }
}
