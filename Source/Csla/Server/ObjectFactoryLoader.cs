//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class containing the default implementation for</summary>
//-----------------------------------------------------------------------

using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Class containing the default implementation for
  /// the FactoryLoader delegate used by the data portal host.
  /// </summary>
  public class ObjectFactoryLoader : IObjectFactoryLoader
  {
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public ObjectFactoryLoader(ApplicationContext applicationContext)
    {
      _applicationContext = Guard.NotNull(applicationContext);
    }

    /// <summary>
    /// Gets the type of the object factory.
    /// </summary>
    /// <param name="factoryName">
    /// Assembly qualified type name for the 
    /// object factory class as provided from 
    /// the ObjectFactory attribute
    /// on the business object.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="factoryName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Type GetFactoryType(string factoryName)
    {
      if (factoryName is null)
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(factoryName)), nameof(factoryName));

      return Type.GetType(factoryName) ?? throw new InvalidOperationException(Resources.FactoryTypeNotFoundException);
    }

    /// <summary>
    /// Creates an instance of an object factory
    /// object for use by the data portal.
    /// </summary>
    /// <param name="factoryName">
    /// Assembly qualified type name for the 
    /// object factory class as provided from 
    /// the ObjectFactory attribute
    /// on the business object.
    /// </param>
    /// <returns>
    /// An instance of the type specified by the
    /// type name parameter.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="factoryName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public object GetFactory(string factoryName)
    {
      if (factoryName is null)
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(factoryName)), nameof(factoryName));

      return _applicationContext.CreateInstanceDI(GetFactoryType(factoryName));
    }
  }
}