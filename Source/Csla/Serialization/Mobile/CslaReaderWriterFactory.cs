//-----------------------------------------------------------------------
// <copyright file="CslaReaderWriterFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory class that is used to create Reader/Writer pair of classes</summary>
//-----------------------------------------------------------------------
using System.Runtime.Serialization;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Factory class that is used to create Reader/Writer pair of classes
  /// to read/write the data during serialization / deserialization process
  /// </summary>
  public static class CslaReaderWriterFactory
  {
    internal static Type ReaderType { get; set; } = typeof(CslaBinaryReader);
    internal static Type WriterType { get; set; } = typeof(CslaBinaryWriter);

    /// <summary>
    /// Get an instance of <see cref="DataContractSerializer"/>
    /// and setup known types for that class
    /// </summary>
    /// <returns>instance of <see cref="DataContractSerializer"/></returns>
    [Obsolete("This type of serialization is unsupported. It will be removed with one of the next major versions of CSLA.NET.")]
    public static DataContractSerializer GetDataContractSerializer()
    {
      return new DataContractSerializer(
        typeof(List<SerializationInfo>),
        new Type[] { typeof(List<int>), typeof(byte[]), typeof(DateTimeOffset), typeof(char[]) });
    }

    /// <summary>
    /// Get an instance of the writer that is used to write data to serialization stream
    /// </summary>
    /// <returns>Instance of the writer that is used to write data to serialization stream</returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public static ICslaWriter GetCslaWriter(ApplicationContext applicationContext)
    {
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      return (ICslaWriter)applicationContext.CreateInstanceDI(WriterType);
    }

    /// <summary>
    /// Get an instance of the reader that is used to read data to serialization stream
    /// </summary>
    /// <returns>Instance of the reader that is used to read data to serialization stream</returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public static ICslaReader GetCslaReader(ApplicationContext applicationContext)
    {
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      return (ICslaReader)applicationContext.CreateInstanceDI(ReaderType);
    }
  }
}
