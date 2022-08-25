#if !NETSTANDARD2_0 && !NET5_0 && !NET6_0
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
  [Obsolete("Use dependency injection", false)]
  public class ServiceClientManager<C, T> : Core.IUseApplicationContext
    where C : System.ServiceModel.ClientBase<T>
    where T : class
  {
    private static object _lock = new object();
    private C _client;
    private string _name = string.Empty;

    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value; }
    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Gets the client proxy object for the
    /// specified name.
    /// </summary>
    /// <param name="name">Unique name for the proxy object.</param>
    /// <returns></returns>
    public ServiceClientManager<C,T > GetManager(string name)
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
      _client = (C)(ApplicationContext.CreateInstanceDI(typeof(C)));
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