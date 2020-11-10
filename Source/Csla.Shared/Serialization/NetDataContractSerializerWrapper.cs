#if !NETFX_CORE && !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="NetDataContractSerializerWrapper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Wraps the <see cref="NetDataContractSerializer"/></summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Csla.Serialization
{
  /// Wraps the <see cref="NetDataContractSerializer"/>
  /// in the 
  /// <see cref="ISerializationFormatter"/>
  /// interface so it can be used in a standardized
  /// manner.
  public class NetDataContractSerializerWrapper : ISerializationFormatter
  {
    private NetDataContractSerializer _formatter =
      new NetDataContractSerializer();

#region ISerializationFormatter Members

    /// <summary>
    /// Converts a serialization stream into an
    /// object graph.
    /// </summary>
    /// <param name="serializationStream">
    /// Byte stream containing the serialized data.</param>
    /// <returns>A deserialized object graph.</returns>
    public object Deserialize(System.IO.Stream serializationStream)
    {
      return _formatter.Deserialize(serializationStream);
    }

    /// <summary>
    /// Converts an object graph into a byte stream.
    /// </summary>
    /// <param name="serializationStream">
    /// Stream that will contain the the serialized data.</param>
    /// <param name="graph">Object graph to be serialized.</param>
    public void Serialize(System.IO.Stream serializationStream, object graph)
    {
      _formatter.Serialize(serializationStream, graph);
    }

#endregion

    /// <summary>
    /// Gets a reference to the underlying
    /// <see cref="NetDataContractSerializer"/>
    /// object.
    /// </summary>
    public NetDataContractSerializer Formatter
    {
      get
      {
        return _formatter;
      }
    }
  }
}
#endif