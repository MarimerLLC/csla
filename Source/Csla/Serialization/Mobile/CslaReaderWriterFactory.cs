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
    private static Type _readerType;
    private static Type _writerType;

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
    /// Setup the type for Writer class
    /// </summary>
    /// <param name="writerType">Type of writer</param>
    public static void SetCslaWriterType(Type writerType)
    {
      _writerType = writerType;
    }

    /// <summary>
    /// Setup the type for reader class
    /// </summary>
    /// <param name="readerType">Reader class type</param>
    public static void SetCslaReaderType(Type readerType)
    {
      _readerType = readerType;
    }

    /// <summary>
    /// Get an instance of the writer that is used to write data to serialization stream
    /// </summary>
    /// <returns>Instance of the writer that is used to write data to serialization stream</returns>
    public  static ICslaWriter GetCslaWriter()
    {
      if (_writerType == null)
      {
        string writerType = Csla.Configuration.ConfigurationManager.AppSettings["CslaWriter"];
        if (string.IsNullOrEmpty(writerType))
        {
          _writerType = typeof(CslaBinaryWriter);
        }
        else
        {
          _writerType = Type.GetType(writerType);
        }
      }
      return (ICslaWriter)Reflection.MethodCaller.CreateInstance(_writerType);
    }

    /// <summary>
    /// Get an instance of the reader that is used to read data to serialization stream
    /// </summary>
    /// <returns>Instance of the reader that is used to read data to serialization stream</returns>
    public  static ICslaReader GetCslaReader()
    {
      if (_readerType == null)
      {
        string readerType = Csla.Configuration.ConfigurationManager.AppSettings["CslaReader"];
        if (string.IsNullOrEmpty(readerType))
        {
          _readerType = typeof(CslaBinaryReader);
        }
        else
        {
          _readerType = Type.GetType(readerType);
        }
      }
      return (ICslaReader)Reflection.MethodCaller.CreateInstance(_readerType);
    }
  }
}
