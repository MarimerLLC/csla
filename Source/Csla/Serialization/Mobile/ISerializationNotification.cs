//-----------------------------------------------------------------------
// <copyright file="ISerializationNotification.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining callback methods used</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Interface defining callback methods used
  /// by the SerializationFormatterFactory.GetFormatter().
  /// </summary>
  public interface ISerializationNotification
  {
    /// <summary>
    /// Method called on an object after deserialization
    /// is complete.
    /// </summary>
    /// <remarks>
    /// This method is called on all objects in an
    /// object graph after all the objects have been
    /// deserialized.
    /// </remarks>
    void Deserialized();
  }
}