//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class containing the default implementation for</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Class containing the default implementation for
  /// the FactoryLoader delegate used by the data portal host.
  /// </summary>
#if NET8_0_OR_GREATER
  [RequiresUnreferencedCode("The factory type with moniker factoryName cannot be statically guessed, and can be trimmed")]
#endif
  public class ObjectFactoryLoader : IObjectFactoryLoader
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public ObjectFactoryLoader(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    private ApplicationContext _applicationContext;

    /// <summary>
    /// Gets the type of the object factory.
    /// </summary>
    /// <param name="factoryName">
    /// Assembly qualified type name for the 
    /// object factory class as provided from 
    /// the ObjectFactory attribute
    /// on the business object.
    /// </param>
#if NET8_0_OR_GREATER
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
#endif
    public Type GetFactoryType(string factoryName)
    {
      return Type.GetType(factoryName);
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
    public object GetFactory(string factoryName)
    {
      var ft = GetFactoryType(factoryName);
      if (ft == null)
        throw new InvalidOperationException(
          string.Format(Resources.FactoryTypeNotFoundException, factoryName));
      return _applicationContext.CreateInstanceDI(ft);
    }
  }
}