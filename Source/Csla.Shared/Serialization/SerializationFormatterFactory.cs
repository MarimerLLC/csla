//-----------------------------------------------------------------------
// <copyright file="SerializationFormatterFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory used to create the appropriate</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using Csla.Reflection;

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
      if (ApplicationContext.SerializationFormatter == ApplicationContext.SerializationFormatters.BinaryFormatter)
        return new BinaryFormatterWrapper();
#if !NETSTANDARD2_0
      else if (ApplicationContext.SerializationFormatter == ApplicationContext.SerializationFormatters.NetDataContractSerializer)
        return new NetDataContractSerializerWrapper();
#endif
      else if (ApplicationContext.SerializationFormatter == ApplicationContext.SerializationFormatters.CustomFormatter)
      {
        string customFormatterTypeName = ConfigurationManager.AppSettings["CslaSerializationFormatter"];
        return (ISerializationFormatter)MethodCaller.CreateInstance(Type.GetType(customFormatterTypeName, true, true));
      }
      else
        return new Csla.Serialization.Mobile.MobileFormatter();
    }

    /// <summary>
    /// <para>
    /// Creates a serialization formatter that is compatible with the platform on which the current process is running.
    /// </para>
    /// <para>
    /// This method will ignore the settings in <see cref="ApplicationContext.SerializationFormatter"/> and
    /// <see cref="ConfigurationManager.AppSettings"/> and should only be used for operations for which data will be
    /// serialized and deserialized in the same process without crossing a data portal boundary (i.e. n-level undo).
    /// </para>
    /// </summary>
    internal static ISerializationFormatter GetNativeFormatter()
    {
      return GetFormatter();
//#if NETSTANDARD2_0
//      return new Csla.Serialization.Mobile.MobileFormatter();
//#else
//      return new NetDataContractSerializerWrapper();
//#endif
    }
  }
}