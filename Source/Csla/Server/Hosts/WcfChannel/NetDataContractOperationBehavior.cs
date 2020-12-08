#if !NETFX_CORE && !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="NetDataContractOperationBehavior.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Override the DataContract serialization behavior to</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel.Description;

namespace Csla.Server.Hosts.WcfChannel
{
  /// <summary>
  /// Override the DataContract serialization behavior to
  /// use the <see cref="NetDataContractSerializer"/>.
  /// </summary>
  public class NetDataContractOperationBehavior : DataContractSerializerOperationBehavior
  {
#region Constructors

    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="operation">Operation description.</param>
    public NetDataContractOperationBehavior(OperationDescription operation)
      : base(operation)
    {
    }

    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="operation">Operation description.</param>
    /// <param name="dataContractFormatAttribute">Data contract attribute object.</param>
    public NetDataContractOperationBehavior(OperationDescription operation, DataContractFormatAttribute dataContractFormatAttribute)
      : base(operation, dataContractFormatAttribute)
    {
    }

#endregion

#region Overrides

    /// <summary>
    /// Overrided CreateSerializer to return an XmlObjectSerializer which is capable of 
    /// preserving the object references.
    /// </summary>
    public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns,
        IList<Type> knownTypes)
    {
      return new NetDataContractSerializer(name, ns);
    }

    /// <summary>
    /// Overrided CreateSerializer to return an XmlObjectSerializer which is capable of 
    /// preserving the object references.
    /// </summary>
    public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name,
        XmlDictionaryString ns, IList<Type> knownTypes)
    {
      return new NetDataContractSerializer(name, ns);
    }

#endregion
  }
}
#endif