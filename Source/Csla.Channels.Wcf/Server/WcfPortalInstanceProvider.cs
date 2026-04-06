//-----------------------------------------------------------------------
// <copyright file="WcfPortalInstanceProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

#if NETFRAMEWORK
using Csla.Server;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Csla.Channels.Wcf.Server
{
  /// <summary>
  /// Represents an object that is used to pass context from a dependency injection container into an instance of
  /// <see cref="WcfPortal"/>.
  /// </summary>
  /// <param name="dataPortal">
  /// The service side data portal that processes the data portal requests.
  /// </param>
  /// <param name="applicationContext">
  /// The server side context for the data portal.
  /// </param>
  internal class WcfPortalInstanceProvider(IDataPortalServer dataPortal, ApplicationContext applicationContext) : IInstanceProvider, IContractBehavior
  {
    #region Implementation of IInstanceProvider

    /// <summary>
    /// Creates a new instance of <see cref="WcfPortal"/>.
    /// </summary>
    /// <param name="instanceContext">
    /// The context information for a WCF service instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="WcfPortal"/>.
    /// </returns>
    public object GetInstance(InstanceContext instanceContext) => new WcfPortal(dataPortal, applicationContext);

    /// <summary>
    /// Creates a new instance of <see cref="WcfPortal"/>.
    /// </summary>
    /// <param name="instanceContext">
    /// The context information for a WCF service instance.
    /// </param>
    /// <param name="message">
    ///  The message that triggered the creation of a service object.
    /// </param>
    /// <returns>
    /// An instance of <see cref="WcfPortal"/>.
    /// </returns>
    public object GetInstance(InstanceContext instanceContext, Message message) => GetInstance(instanceContext);

    #region Unused
    public void ReleaseInstance(InstanceContext instanceContext, object instance) { }
    #endregion

    #endregion

    #region IContractBehavior

    /// <summary>
    /// Sets the <see cref="DispatchRuntime.InstanceProvider"/> to this instance of <see cref="WcfPortalInstanceProvider"/>.
    /// </summary>
    /// <param name="contractDescription">
    /// The contract description to be modified.
    /// </param>
    /// <param name="endpoint">
    /// The endpoint to modify.
    /// </param>
    /// <param name="dispatchRuntime">
    /// The dispatch runtime that controls service execution.
    /// </param>
    public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime) =>
      dispatchRuntime.InstanceProvider = this;

    #region Unused
    public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
    public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
    public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint) { }
    #endregion

    #endregion
  }
}
#endif
