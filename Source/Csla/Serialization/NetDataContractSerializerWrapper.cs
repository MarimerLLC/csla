#if !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="NetDataContractSerializerWrapper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Wraps the <see cref="NetDataContractSerializer"/></summary>
//-----------------------------------------------------------------------
using System.IO;
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
    private readonly NetDataContractSerializer _formatter =
      new NetDataContractSerializer();

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
    /// Converts a serialization stream into an
    /// object graph.
    /// </summary>
    /// <param name="buffer">
    /// Byte stream containing the serialized data.</param>
    /// <returns>A deserialized object graph.</returns>
    public object Deserialize(byte[] buffer)
    {
      using var serializationStream = new MemoryStream(buffer);
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

    /// <summary>
    /// Converts an object graph into a byte stream.
    /// </summary>
    /// <param name="graph">Object graph to be serialized.</param>
    public byte[] Serialize(object graph)
    {
      using var buffer = new MemoryStream();
      _formatter.Serialize(buffer, graph);
      buffer.Position = 0;
      return buffer.ToArray();
    }

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