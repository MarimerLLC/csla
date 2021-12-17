//-----------------------------------------------------------------------
// <copyright file="SerializationFormatterFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory used to create the appropriate</summary>
//-----------------------------------------------------------------------
namespace Csla.Serialization
{
  /// <summary>
  /// Factory used to create the appropriate
  /// serialization formatter object based
  /// on the application configuration.
  /// </summary>
  public static class SerializationFormatterFactory
  {
    /// <summary>
    /// Creates a serialization formatter object.
    /// </summary>
    public static ISerializationFormatter GetFormatter(ApplicationContext applicationContext)
    {
      return applicationContext.CreateInstanceDI<ISerializationFormatter>(ApplicationContext.SerializationFormatter);
    }
  }
}