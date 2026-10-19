//-----------------------------------------------------------------------
// <copyright file="CslaReaderWriterFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory class that is used to create Reader/Writer pair of classes</summary>
//-----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Factory class that is used to create Reader/Writer pair of classes
  /// to read/write the data during serialization / deserialization process
  /// </summary>
  public static class CslaReaderWriterFactory
  {
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal static Type ReaderType { get; set; } = typeof(CslaBinaryReader);
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal static Type WriterType { get; set; } = typeof(CslaBinaryWriter);

    /// <summary>
    /// Get an instance of the writer that is used to write data to serialization stream
    /// </summary>
    /// <returns>Instance of the writer that is used to write data to serialization stream</returns>
    /// <param name="applicationContext"></param>
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
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public static ICslaReader GetCslaReader(ApplicationContext applicationContext)
    {
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      return (ICslaReader)applicationContext.CreateInstanceDI(ReaderType);
    }
  }
}
