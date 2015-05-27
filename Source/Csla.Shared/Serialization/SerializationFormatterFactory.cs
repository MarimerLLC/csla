//-----------------------------------------------------------------------
// <copyright file="SerializationFormatterFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Factory used to create the appropriate</summary>
//-----------------------------------------------------------------------
using System;

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
    public static ISerializationFormatter GetFormatter()
    {
#if SILVERLIGHT || NETFX_CORE
      return new Csla.Serialization.Mobile.MobileFormatter();
#else
      if (ApplicationContext.SerializationFormatter == ApplicationContext.SerializationFormatters.BinaryFormatter)
        return new BinaryFormatterWrapper();
      else if (ApplicationContext.SerializationFormatter == ApplicationContext.SerializationFormatters.NetDataContractSerializer)
        return new NetDataContractSerializerWrapper();
      else
        return new Csla.Serialization.Mobile.MobileFormatter();
#endif
    }
  }
}