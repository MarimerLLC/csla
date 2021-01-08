#if !NETFX_PHONE && !NETCORE && !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="ServiceClientManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides an automated way to reuse </summary>
//-----------------------------------------------------------------------
using System;

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
        ServiceClientManager<C, T> mgr = (ServiceClientManager<C, T>)ApplicationContext.LocalContext.GetValueOrNull(name);
        if (mgr == null)
        {
          mgr = new ServiceClientManager<C, T>(name);
          ApplicationContext.LocalContext[name] = mgr;
        }
        return mgr;
      }
    }


    private ServiceClientManager(string name)
    {
      _client = (C)(Reflection.MethodCaller.CreateInstance(typeof(C)));
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
#endif