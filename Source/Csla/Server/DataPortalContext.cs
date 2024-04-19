//-----------------------------------------------------------------------
// <copyright file="DataPortalContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;
using Csla.Core;
using Csla.Configuration;

namespace Csla.Server
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  [Serializable]
  public class DataPortalContext : Csla.Serialization.Mobile.IMobileObject, IUseApplicationContext
  {
    [NonSerialized]
    private TransactionalTypes _transactionalType;
    [NonSerialized]
    private ObjectFactoryAttribute _factoryInfo;

    /// <summary>
    /// The current principal object
    /// if CSLA security is being used.
    /// </summary>
    public IPrincipal Principal { get; private set; }

    /// <summary>
    /// Returns true if the 
    /// server-side DataPortal is running
    /// on a remote server via remoting.
    /// </summary>
    public bool IsRemotePortal { get; private set; }

    /// <summary>
    /// The culture setting on the client
    /// workstation.
    /// </summary>
    public string ClientCulture { get; private set; }

    /// <summary>
    /// The culture setting on the client
    /// workstation.
    /// </summary>
    public string ClientUICulture { get; private set; }

    internal ContextDictionary ClientContext { get; private set; }

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

    private ApplicationContext _applicationContext;
    ApplicationContext IUseApplicationContext.ApplicationContext { get => _applicationContext; set => _applicationContext = value; }

    /// <summary>
    /// Creates a new DataPortalContext object.
    /// </summary>
    /// <param name="applicationContext">ApplicationContext instance.</param>
    /// <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    public DataPortalContext(ApplicationContext applicationContext, bool isRemotePortal)
    {
      _applicationContext = applicationContext;
      Principal = GetPrincipal(applicationContext, isRemotePortal);
      IsRemotePortal = isRemotePortal;
      ClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      ClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
      ClientContext = applicationContext.ContextManager.GetClientContext(applicationContext.ExecutionLocation);
    }

    /// <summary>
    /// Creates a new DataPortalContext object.
    /// </summary>
    /// <param name="applicationContext">ApplicationContext instance.</param>
    /// <param name="principal">The current Principal object.</param>
    /// <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    /// <param name="clientContext">Client context.</param>
    /// <param name="clientCulture">Client culture.</param>
    /// <param name="clientUICulture">Client UI culture.</param>
    public DataPortalContext(ApplicationContext applicationContext, IPrincipal principal, bool isRemotePortal, string clientCulture, string clientUICulture, ContextDictionary clientContext)
    {
      _applicationContext = applicationContext;
      Principal = principal;
      ClientContext = clientContext;
      ClientCulture = clientCulture;
      ClientUICulture = clientUICulture;
      IsRemotePortal = isRemotePortal;
    }

    /// <summary>
    /// Default constructor for use by SerializationFormatterFactory.GetFormatter().
    /// </summary>
    public DataPortalContext()
    { }

    private IPrincipal GetPrincipal(ApplicationContext applicationContext, bool isRemotePortal)
    {
      var securityOptions = applicationContext.GetRequiredService<SecurityOptions>();
      if (isRemotePortal && !securityOptions.FlowSecurityPrincipalFromClient)
      {
        // Platform-supplied security (including Windows and ASP.NET)
        return null;
      }
      else
      {
        // we assume using the CSLA framework security
        return applicationContext.User;
      }
    }

    void Serialization.Mobile.IMobileObject.GetState(Serialization.Mobile.SerializationInfo info)
    {
      info.AddValue("principal", Csla.Serialization.SerializationFormatterFactory.GetFormatter(_applicationContext).Serialize(Principal));
      info.AddValue("clientContext", Csla.Serialization.SerializationFormatterFactory.GetFormatter(_applicationContext).Serialize(ClientContext));
      info.AddValue("clientCulture", ClientCulture);
      info.AddValue("clientUICulture", ClientUICulture);
      info.AddValue("isRemotePortal", IsRemotePortal);
    }

    void Serialization.Mobile.IMobileObject.GetChildren(Serialization.Mobile.SerializationInfo info, Serialization.Mobile.MobileFormatter formatter)
    {
    }

    void Serialization.Mobile.IMobileObject.SetState(Serialization.Mobile.SerializationInfo info)
    {
      Principal = (IPrincipal)Csla.Serialization.SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(info.GetValue<byte[]>("principal"));
      ClientContext = (ContextDictionary)Csla.Serialization.SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(info.GetValue<byte[]>("clientContext"));
      ClientCulture = info.GetValue<string>("clientCulture");
      ClientUICulture = info.GetValue<string>("clientUICulture");
      IsRemotePortal = info.GetValue<bool>("isRemotePortal");
    }

    void Serialization.Mobile.IMobileObject.SetChildren(Serialization.Mobile.SerializationInfo info, Serialization.Mobile.MobileFormatter formatter)
    {
    }
  }
}
