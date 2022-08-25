using System;
using System.Collections.Generic;
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
    /// <param name="applicationContext"></param>
    public static ICslaWriter GetCslaWriter(ApplicationContext applicationContext)
    {
      return (ICslaWriter)applicationContext.CreateInstanceDI(WriterType);
    }

    /// <summary>
    /// Get an instance of the reader that is used to read data to serialization stream
    /// </summary>
    /// <returns>Instance of the reader that is used to read data to serialization stream</returns>
    /// <param name="applicationContext"></param>
    public static ICslaReader GetCslaReader(ApplicationContext applicationContext)
    {
      return (ICslaReader)applicationContext.CreateInstanceDI(ReaderType);
    }
  }
}
