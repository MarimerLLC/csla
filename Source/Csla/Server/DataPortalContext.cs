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
using Csla.Serialization;

namespace Csla.Server
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  [Serializable]
  public class DataPortalContext : Serialization.Mobile.IMobileObject, IUseApplicationContext
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

    internal IContextDictionary ClientContext { get; private set; }

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

    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext { get => _applicationContext; set => _applicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext)); }

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
      ClientCulture = Thread.CurrentThread.CurrentCulture.Name;
      ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name;
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
    public DataPortalContext(ApplicationContext applicationContext, IPrincipal principal, bool isRemotePortal, string clientCulture, string clientUICulture, IContextDictionary clientContext)
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
      info.AddValue("principal", _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(Principal));
      info.AddValue("clientContext", _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(ClientContext));
      info.AddValue("clientCulture", ClientCulture);
      info.AddValue("clientUICulture", ClientUICulture);
      info.AddValue("isRemotePortal", IsRemotePortal);
    }

    void Serialization.Mobile.IMobileObject.GetChildren(Serialization.Mobile.SerializationInfo info, Serialization.Mobile.MobileFormatter formatter)
    {
    }

    void Serialization.Mobile.IMobileObject.SetState(Serialization.Mobile.SerializationInfo info)
    {
      Principal = (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(info.GetValue<byte[]>("principal"));
      ClientContext = (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(info.GetValue<byte[]>("clientContext"));
      ClientCulture = info.GetValue<string>("clientCulture");
      ClientUICulture = info.GetValue<string>("clientUICulture");
      IsRemotePortal = info.GetValue<bool>("isRemotePortal");
    }

    void Serialization.Mobile.IMobileObject.SetChildren(Serialization.Mobile.SerializationInfo info, Serialization.Mobile.MobileFormatter formatter)
    {
    }
  }
}
