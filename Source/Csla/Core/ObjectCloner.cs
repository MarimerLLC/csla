//-----------------------------------------------------------------------
// <copyright file="ObjectCloner.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This class provides an implementation of a deep</summary>
//-----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// This class provides an implementation of a deep
  /// clone of a complete object graph. Objects are
  /// copied at the field level.
  /// </summary>
  public class ObjectCloner
  {
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public ObjectCloner(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Gets an instance of ObjectCloner.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
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
    /// specified in ApplicationContext.</para>
    /// <para>The default is to use the MobileFormatter.</para>
    /// </remarks>
    [return: NotNullIfNotNull(nameof(obj))]
    public object? Clone(object? obj)
    {
      using var buffer = new MemoryStream();
      ISerializationFormatter formatter = _applicationContext.GetRequiredService<ISerializationFormatter>();
      formatter.Serialize(buffer, obj);
      buffer.Position = 0;
      return formatter.Deserialize(buffer);
    }
  }
}