using System;
using System.Security.Principal;
using System.Threading;
using Csla.Core;

namespace Csla.Windows
{
  /// <summary>
  /// ApplicationContextManager for Windows Forms applications
  /// </summary>
  public class ApplicationContextManager : IContextManager
  {
    private const string _localContextName = "Csla.LocalContext";
    private const string _clientContextName = "Csla.ClientContext";
    private const string _globalContextName = "Csla.GlobalContext";

    private static IPrincipal _principal;

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    public bool IsValid
    {
      get { return true; }
    }

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    /// <returns></returns>
    public IPrincipal GetUser()
    {
      IPrincipal current;
      if (_principal == null)
      {
        if (ApplicationContext.AuthenticationType != "Windows")
          _principal = new Csla.Security.UnauthenticatedPrincipal();
        else
          _principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
      }
      current = _principal;
      return current;
    }

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public void SetUser(IPrincipal principal)
    {
      _principal = principal;
      Thread.CurrentPrincipal = principal;
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    /// <returns></returns>
    public ContextDictionary GetLocalContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      return (ContextDictionary)Thread.GetData(slot);
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      Thread.SetData(slot, localContext);
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
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

    /// <summary>
    /// Gets the global context.
    /// </summary>
    /// <returns></returns>
    public ContextDictionary GetGlobalContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
      return (ContextDictionary)Thread.GetData(slot);
    }

    /// <summary>
    /// Sets the global context.
    /// </summary>
    /// <param name="globalContext">Global context.</param>
    public void SetGlobalContext(ContextDictionary globalContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
      Thread.SetData(slot, globalContext);
    }
  }
}