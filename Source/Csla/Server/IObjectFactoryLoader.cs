//-----------------------------------------------------------------------
// <copyright file="IObjectFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines an interface to be implemented by</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Defines an interface to be implemented by
  /// a factory loader object that returns ObjectFactory
  /// objects based on the ObjectFactory attributes
  /// used to decorate CSLA .NET business objects.
  /// </summary>
  public interface IObjectFactoryLoader
  {
    /// <summary>
    /// Returns the type of the factory object.
    /// </summary>
    /// <param name="factoryName">
    /// Name of the factory to create, typically
    /// an assembly qualified type name.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="factoryName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    Type GetFactoryType(string factoryName);
    /// <summary>
    /// Returns an ObjectFactory object.
    /// </summary>
    /// <param name="factoryName">
    /// Name of the factory to create, typically
    /// an assembly qualified type name.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="factoryName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    object GetFactory(string factoryName);
  }
}