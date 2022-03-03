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
  public class TestContextManager : IContextManager
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

    public bool IsStatefulContext => true;

    public ApplicationContext ApplicationContext { get; set; }

    public IPrincipal GetUser()
    {
      IPrincipal result = Thread.CurrentPrincipal;
      if (result == null)
      {
        result = new System.Security.Claims.ClaimsPrincipal();
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

    public ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      if (_myContext[_clientContextName] == null)
        SetClientContext(new ContextDictionary(), executionLocation);
      return (ContextDictionary) _myContext[_clientContextName];
    }

    public void SetClientContext(ContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
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
    /// Gets the service provider for current scope
    /// </summary>
    /// <returns></returns>
    public IServiceProvider GetServiceProvider()
    {
      return (IServiceProvider)ApplicationContext.LocalContext["__sps"];
    }

    /// <summary>
    /// Sets the service provider for current scope
    /// </summary>
    /// <param name="scope">IServiceProvider instance</param>
    public void SetServiceProvider(IServiceProvider scope)
    {
      ApplicationContext.LocalContext["__sps"] = scope;
    }
  }
}
