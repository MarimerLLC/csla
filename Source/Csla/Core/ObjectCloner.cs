//-----------------------------------------------------------------------
// <copyright file="ObjectCloner.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This class provides an implementation of a deep</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;
using System;
using System.IO;

namespace Csla.Core
{
  /// <summary>
  /// This class provides an implementation of a deep
  /// clone of a complete object graph. Objects are
  /// copied at the field level.
  /// </summary>
  public class ObjectCloner
  {
    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public ObjectCloner(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
    }

    /// <summary>
    /// Gets an instance of ObjectCloner.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <returns></returns>
    public static ObjectCloner GetInstance(ApplicationContext applicationContext)
    {
      return new ObjectCloner(applicationContext);
    }

    /// <summary>
    /// Clones an object.
    /// </summary>
    /// <param name="obj">The object to clone.</param>
    /// <remarks>
    /// <para>The object to be cloned must be serializable.</para>
    /// <para>The serialization is performed using the formatter
    /// specified in the application's configuration file
    /// using the CslaSerializationFormatter key.</para>
    /// <para>In full .NET the default is to use the 
    /// System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
    /// </para>. You may also choose to use the Microsoft .NET 3.0
    /// System.Runtime.Serialization.NetDataContractSerializer
    /// NetDataContractSerializer provided as part of WCF.
    /// <para>In UWP and Blazor the only
    /// option is to use the CSLA .NET MobileFormatter
    /// for serialization as the other serializers don't
    /// exist.</para>
    /// </remarks>
    public object Clone(object obj)
    {
      using var buffer = new MemoryStream();
      ISerializationFormatter formatter =
        SerializationFormatterFactory.GetFormatter(ApplicationContext);
      formatter.Serialize(buffer, obj);
      buffer.Position = 0;
      return formatter.Deserialize(buffer);
    }
  }
}