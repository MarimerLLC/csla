//-----------------------------------------------------------------------
// <copyright file="IObjectFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines an interface to be implemented by</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    Type GetFactoryType(string factoryName);
    /// <summary>
    /// Returns an ObjectFactory object.
    /// </summary>
    /// <param name="factoryName">
    /// Name of the factory to create, typically
    /// an assembly qualified type name.
    /// </param>
    object GetFactory(string factoryName);
  }
}