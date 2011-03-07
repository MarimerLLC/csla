using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Csla.Core;

namespace Csla.Xaml
{
  /// <summary>
  /// ApplicationContextManager for WPF applications
  /// </summary>
  public class ApplicationContextManager : IContextManager
  {
    private const string _localContextName = "Csla.LocalContext";
    private const string _clientContextName = "Csla.ClientContext";
    private const string _globalContextName = "Csla.GlobalContext";

    private static IPrincipal _principal;

    public bool IsValid
    {
      get { return true; }
    }

    public IPrincipal GetUser()
    {
      IPrincipal current;
      if (System.Windows.Application.Current != null)
      {
        if (_principal == null)
        {
          if (ApplicationContext.AuthenticationType != "Windows")
            _principal = new Csla.Security.UnauthenticatedPrincipal();
          else
            _principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        }
        current = _principal;
      }
      else
        current = Thread.CurrentPrincipal;
      return current;
    }

    public void SetUser(IPrincipal principal)
    {
      if (System.Windows.Application.Current != null)
        _principal = principal;
      Thread.CurrentPrincipal = principal;
    }

    public ContextDictionary GetLocalContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      return (ContextDictionary)Thread.GetData(slot);
    }

    public void SetLocalContext(ContextDictionary localContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      Thread.SetData(slot, localContext);
    }

    public ContextDictionary GetClientContext()
    {
      if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Client)
      {
        return (ContextDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
      }
      else
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
        return (ContextDictionary)Thread.GetData(slot);
      }
    }

    public void SetClientContext(ContextDictionary clientContext)
    {
      if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Client)
      {
        AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
      }
      else
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
        Thread.SetData(slot, clientContext);
      }
    }

    public ContextDictionary GetGlobalContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
      return (ContextDictionary)Thread.GetData(slot);
    }

    public void SetGlobalContext(ContextDictionary globalContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
      Thread.SetData(slot, globalContext);
    }
  }

 
}
