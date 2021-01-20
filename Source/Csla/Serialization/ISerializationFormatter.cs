//-----------------------------------------------------------------------
// <copyright file="ISerializationFormatter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines an object that can serialize and deserialize</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization
{
  /// <summary>
  /// Defines an object that can serialize and deserialize
  /// object graphs.
  /// </summary>
  public interface ISerializationFormatter
  {
    /// <summary>
    /// Converts a serialization stream into an
    /// object graph.
    /// </summary>
    /// <param name="serializationStream">
    /// Byte stream containing the serialized data.</param>
    /// <returns>A deserialized object graph.</returns>
    object Deserialize(System.IO.Stream serializationStream);
    /// <summary>
    /// Converts a serialization stream into an
    /// object graph.
    /// </summary>
    /// <param name="serializationStream">
    /// Byte stream containing the serialized data.</param>
    /// <returns>A deserialized object graph.</returns>
    object Deserialize(byte[] serializationStream);
    /// <summary>
    /// Converts an object graph into a byte stream.
    /// </summary>
    /// <param name="serializationStream">
    /// Stream that will contain the the serialized data.</param>
    /// <param name="graph">Object graph to be serialized.</param>
    void Serialize(System.IO.Stream serializationStream, object graph);
    /// <summary>
    /// Converts an object graph into a byte stream.
    /// </summary>
    /// <param name="graph">Object graph to be serialized.</param>
    byte[] Serialize(object graph);
  }
}