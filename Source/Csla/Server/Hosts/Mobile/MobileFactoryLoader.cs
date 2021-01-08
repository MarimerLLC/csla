//-----------------------------------------------------------------------
// <copyright file="MobileFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class containing the default implementation for</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Class containing the default implementation for
  /// the FactoryLoader delegate used by the
  /// data portal host.
  /// </summary>
  public class MobileFactoryLoader : IMobileFactoryLoader
  {
    /// <summary>
    /// Creates an instance of a mobile factory
    /// object for use by the data portal.
    /// </summary>
    /// <param name="factoryName">
    /// Type assembly qualified type name for the 
    /// mobile factory class as
    /// provided from the MobileFactory attribute
    /// on the business object.
    /// </param>
    /// <returns>
    /// An instance of the type specified by the
    /// type name parameter.
    /// </returns>
    public object GetFactory(string factoryName)
    {
      var ft = Type.GetType(factoryName);
      if (ft == null)
        throw new InvalidOperationException(
          string.Format(Resources.FactoryTypeNotFoundException, factoryName));
      return Reflection.MethodCaller.CreateInstance(ft);
    }
  }
}