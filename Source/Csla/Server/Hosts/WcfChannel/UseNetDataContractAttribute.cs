#if !NETFX_CORE && !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="UseNetDataContractAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Specify that WCF should serialize objects in a .NET</summary>
//-----------------------------------------------------------------------
using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Csla.Server.Hosts.WcfChannel
{
  /// <summary>
  /// Specify that WCF should serialize objects in a .NET
  /// specific manner to as to preserve complex object
  /// references and to be able to deserialize the graph
  /// into the same type as the original objets.
  /// </summary>
  public class UseNetDataContractAttribute : Attribute, IOperationBehavior
  {
#region IOperationBehavior Members

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="description">Not implemented.</param>
    /// <param name="parameters">Not implemented.</param>
    public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
    {
    }

    /// <summary>
    /// Apply the client behavior by requiring
    /// the use of the NetDataContractSerializer.
    /// </summary>
    /// <param name="description">Operation description.</param>
    /// <param name="proxy">Client operation object.</param>
    public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
    {
      ReplaceDataContractSerializerOperationBehavior(description);
    }

    /// <summary>
    /// Apply the dispatch behavior by requiring
    /// the use of the NetDataContractSerializer.
    /// </summary>
    /// <param name="description">Operation description.</param>
    /// <param name="dispatch">Dispatch operation object.</param>
    public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
    {
      ReplaceDataContractSerializerOperationBehavior(description);
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="description">Not implemented.</param>
    public void Validate(OperationDescription description)
    {
    }

#endregion

    private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description)
    {
      DataContractSerializerOperationBehavior dcsOperationBehavior = description.Behaviors.Find<DataContractSerializerOperationBehavior>();

      if (dcsOperationBehavior != null)
      {
        description.Behaviors.Remove(dcsOperationBehavior);
        description.Behaviors.Add(new NetDataContractOperationBehavior(description));
      }
    }
  }
}
#endif