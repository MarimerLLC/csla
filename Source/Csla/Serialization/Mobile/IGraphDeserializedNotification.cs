//-----------------------------------------------------------------------
// <copyright file="IGraphDeserializedNotification.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Callback raised after an entire object graph is deserialized</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Callback raised by <see cref="MobileFormatter"/> after every object in
  /// the graph has been deserialized and has received its
  /// <see cref="ISerializationNotification.Deserialized"/> call.
  /// </summary>
  /// <remarks>
  /// Objects are notified in descending reference order, so child objects
  /// are notified before the objects that contain them.
  /// </remarks>
  internal interface IGraphDeserializedNotification
  {
    /// <summary>
    /// Called after the entire object graph has been deserialized.
    /// </summary>
    void GraphDeserialized();
  }
}
