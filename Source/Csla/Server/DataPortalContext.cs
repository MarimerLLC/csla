//-----------------------------------------------------------------------
// <copyright file="DataPortalContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Collections.Specialized;
using Csla.Core;

namespace Csla.Server
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  [Serializable]
  public class DataPortalContext : Csla.Serialization.Mobile.IMobileObject
  {
    private IPrincipal _principal;
    private bool _remotePortal;
    private string _clientCulture;
    private string _clientUICulture;
    private ContextDictionary _clientContext;
    private ContextDictionary _globalContext;
    [NonSerialized]
    private TransactionalTypes _transactionalType;
    [NonSerialized]
    private ObjectFactoryAttribute _factoryInfo;

    /// <summary>
    /// The current principal object
    /// if CSLA security is being used.
    /// </summary>
    public IPrincipal Principal
    {
      get { return _principal; }
    }

    /// <summary>
    /// Returns true if the 
    /// server-side DataPortal is running
    /// on a remote server via remoting.
    /// </summary>
    public bool IsRemotePortal
    {
      get { return _remotePortal; }
    }

    /// <summary>
    /// The culture setting on the client
    /// workstation.
    /// </summary>
    public string ClientCulture
    {
      get { return _clientCulture; }
    }

    /// <summary>
    /// The culture setting on the client
    /// workstation.
    /// </summary>
    public string ClientUICulture
    {
      get { return _clientUICulture; }
    }
    internal ContextDictionary ClientContext
    {
      get { return _clientContext; }
    }

    internal ContextDictionary GlobalContext
    {
      get { return _globalContext; }
    }

    /// <summary>
    /// Gets the current transactional type. Only valid
    /// in the server-side data portal methods after
    /// the transactional type has been determined.
    /// </summary>
    public TransactionalTypes TransactionalType
    {
      get { return _transactionalType; }
      internal set { _transactionalType = value; }
    }

    /// <summary>
    /// Gets the current ObjectFactory attribute
    /// value (if any). Only valid in the server-side
    /// data portal methods after the attribute
    /// value has been determined.
    /// </summary>
    public ObjectFactoryAttribute FactoryInfo
    {
      get { return _factoryInfo; }
      internal set { _factoryInfo = value; }
    }

    /// <summary>
    /// Creates a new DataPortalContext object.
    /// </summary>
    /// <param name="principal">The current Principal object.</param>
    /// <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    public DataPortalContext(IPrincipal principal, bool isRemotePortal)
    {
      if (isRemotePortal)
      {
        _principal = principal;
        _remotePortal = isRemotePortal;
#if NETFX_CORE
        _clientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
        _clientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
#else
        _clientCulture = 
          System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        _clientUICulture = 
          System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
#endif
        _clientContext = Csla.ApplicationContext.ContextManager.GetClientContext();
        _globalContext = Csla.ApplicationContext.ContextManager.GetGlobalContext();
      }
      else if (ApplicationContext.WebContextManager != null && ApplicationContext.WebContextManager.IsValid)
      {
        _clientContext = Csla.ApplicationContext.ContextManager.GetClientContext();
        _globalContext = Csla.ApplicationContext.ContextManager.GetGlobalContext();
      }
    }

    /// <summary>
    /// Creates a new DataPortalContext object.
    /// </summary>
    /// <param name="principal">The current Principal object.</param>
    /// <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    /// <param name="clientContext">Client context.</param>
    /// <param name="clientCulture">Client culture.</param>
    /// <param name="clientUICulture">Client UI culture.</param>
    /// <param name="globalContext">Global context.</param>
    public DataPortalContext(IPrincipal principal, bool isRemotePortal, string clientCulture, string clientUICulture, ContextDictionary clientContext, ContextDictionary globalContext)
    {
      _principal = principal;
      _clientContext = clientContext;
      _clientCulture = clientCulture;
      _clientUICulture = clientUICulture;
      _globalContext = globalContext;
      _remotePortal = isRemotePortal;
    }

    /// <summary>
    /// Default constructor for use by SerializationFormatterFactory.GetFormatter().
    /// </summary>
    public DataPortalContext()
    { }

    void Serialization.Mobile.IMobileObject.GetState(Serialization.Mobile.SerializationInfo info)
    {
      info.AddValue("principal", Csla.Serialization.SerializationFormatterFactory.GetFormatter().Serialize(_principal));
      info.AddValue("clientContext", Csla.Serialization.SerializationFormatterFactory.GetFormatter().Serialize(_clientContext));
      info.AddValue("clientCulture", _clientCulture);
      info.AddValue("clientUICulture", _clientUICulture);
      info.AddValue("globalContext", Csla.Serialization.SerializationFormatterFactory.GetFormatter().Serialize(_globalContext));
      info.AddValue("isRemotePortal", _remotePortal);
    }

    void Serialization.Mobile.IMobileObject.GetChildren(Serialization.Mobile.SerializationInfo info, Serialization.Mobile.MobileFormatter formatter)
    {
    }

    void Serialization.Mobile.IMobileObject.SetState(Serialization.Mobile.SerializationInfo info)
    {
      _principal = (IPrincipal)Csla.Serialization.SerializationFormatterFactory.GetFormatter().Deserialize(info.GetValue<byte[]>("principal"));
      _clientContext = (ContextDictionary)Csla.Serialization.SerializationFormatterFactory.GetFormatter().Deserialize(info.GetValue<byte[]>("clientContext"));
      _clientCulture = info.GetValue<string>("clientCulture");
      _clientUICulture = info.GetValue<string>("clientUICulture");
      _globalContext = (ContextDictionary)Csla.Serialization.SerializationFormatterFactory.GetFormatter().Deserialize(info.GetValue<byte[]>("globalContext")); ;
      _remotePortal = info.GetValue<bool>("isRemotePortal");
    }

    void Serialization.Mobile.IMobileObject.SetChildren(Serialization.Mobile.SerializationInfo info, Serialization.Mobile.MobileFormatter formatter)
    {
    }
  }
}
