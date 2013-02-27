using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Csla.Core;

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
      IPrincipal current = Thread.CurrentPrincipal;
      return current;
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
  }
}
