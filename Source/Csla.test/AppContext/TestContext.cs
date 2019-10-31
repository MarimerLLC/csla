using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Test.AppContext
{
  public class TestContext : IContextManager
  {
    [ThreadStatic]
    private static HybridDictionary _myContext = new HybridDictionary();

    private const string _localContextName = "Csla.ClientContext";
    private const string _clientContextName = "Csla.ClientContext";
    private const string _globalContextName = "Csla.GlobalContext";

    public bool IsValid
    {
      get { return true; }
    }

    public IPrincipal GetUser()
    {
      IPrincipal result = Thread.CurrentPrincipal;
      if (result == null)
      {
        result = new Csla.Security.UnauthenticatedPrincipal();
        SetUser(result);
      }
      return result;
    }

    public void SetUser(IPrincipal principal)
    {
      Thread.CurrentPrincipal = principal;
    }

    public ContextDictionary GetLocalContext()
    {
      if (_myContext[_localContextName] == null)
        SetLocalContext(new ContextDictionary());
      return (ContextDictionary)_myContext[_localContextName];
    }

    public void SetLocalContext(ContextDictionary localContext)
    {
      _myContext[_localContextName] = localContext;
    }

    public ContextDictionary GetClientContext()
    {
      if (_myContext[_clientContextName] == null)
        SetClientContext(new ContextDictionary());
      return (ContextDictionary) _myContext[_clientContextName];
    }

    public void SetClientContext(ContextDictionary clientContext)
    {
      _myContext[_clientContextName] = clientContext;
    }

    public ContextDictionary GetGlobalContext()
    {
      if (_myContext[_globalContextName] == null)
        SetGlobalContext(new ContextDictionary());
      return (ContextDictionary)_myContext[_globalContextName];
    }

    public void SetGlobalContext(ContextDictionary globalContext)
    {
      _myContext[_globalContextName] = globalContext;
    }

    private static IServiceProvider _provider;

    /// <summary>
    /// Gets the default IServiceProvider
    /// </summary>
    public IServiceProvider GetDefaultServiceProvider()
    {
      return _provider;
    }

    /// <summary>
    /// Sets the default IServiceProvider
    /// </summary>
    /// <param name="serviceProvider">IServiceProvider instance</param>
    public void SetDefaultServiceProvider(IServiceProvider serviceProvider)
    {
      _provider = serviceProvider;
    }

    /// <summary>
    /// Gets the scoped IServiceProvider
    /// </summary>
    public IServiceProvider GetScopedServiceProvider()
    {
      IServiceProvider result;
      result = (IServiceProvider)Csla.ApplicationContext.LocalContext["__ssp"];
      if (result == null)
        result = GetDefaultServiceProvider();
      return result;
    }

    /// <summary>
    /// Sets the scoped IServiceProvider
    /// </summary>
    /// <param name="serviceProvider">IServiceProvider instance</param>
    public void SetScopedServiceProvider(IServiceProvider serviceProvider)
    {
      Csla.ApplicationContext.LocalContext["__ssp"] = serviceProvider;
    }

    /// <summary>
    /// Gets the service provider scope
    /// </summary>
    /// <returns></returns>
    public IServiceScope GetServiceProviderScope()
    {
      return (IServiceScope)ApplicationContext.LocalContext["__sps"];
    }

    /// <summary>
    /// Sets the service provider scope
    /// </summary>
    /// <param name="scope">IServiceScope instance</param>
    public void SetServiceProviderScope(IServiceScope scope)
    {
      Csla.ApplicationContext.LocalContext["__sps"] = scope;
    }
  }
}
