#if !NET6_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="BinaryFormatterWrapper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Wraps the <see cref="BinaryFormatter"/></summary>
//-----------------------------------------------------------------------
using System.Runtime.Serialization.Formatters.Binary;

namespace Csla.Serialization
{
  /// <summary>
  /// Wraps the <see cref="BinaryFormatter"/>
  /// in the 
  /// <see cref="ISerializationFormatter"/>
  /// interface so it can be used in a standardized
  /// manner.
  /// </summary>
  public class BinaryFormatterWrapper : ISerializationFormatter
  {
    /// <summary>
    /// Converts a serialization stream into an
    /// object graph.
    /// </summary>
    /// <param name="serializationStream">
    /// Byte stream containing the serialized data.</param>
    /// <returns>A deserialized object graph.</returns>
    public object Deserialize(System.IO.Stream serializationStream)
    {
      return Formatter.Deserialize(serializationStream);
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
      return Formatter.Deserialize(serializationStream);
    }

    /// <summary>
    /// Converts an object graph into a byte stream.
    /// </summary>
    /// <param name="serializationStream">
    /// Stream that will contain the the serialized data.</param>
    /// <param name="graph">Object graph to be serialized.</param>
    public void Serialize(System.IO.Stream serializationStream, object graph)
    {
      Formatter.Serialize(serializationStream, graph);
    }

    /// <summary>
    /// Converts an object graph into a byte stream.
    /// </summary>
    /// <param name="graph">Object graph to be serialized.</param>
    public byte[] Serialize(object graph)
    {
      using var buffer = new MemoryStream();
      Formatter.Serialize(buffer, graph);
      buffer.Position = 0;
      return buffer.ToArray();
    }

    /// <summary>
    /// Gets a reference to the underlying
    /// <see cref="BinaryFormatter"/>
    /// object.
    /// </summary>
    public BinaryFormatter Formatter { get; } = new BinaryFormatter();
  }
}
#endif