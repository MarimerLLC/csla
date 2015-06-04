//-----------------------------------------------------------------------
// <copyright file="PropertyChangingEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Arguments object containing information about</summary>
//-----------------------------------------------------------------------
#if (ANDROID || IOS) || __ANDROID__ || IOS || SILVERLIGHT4 || NETFX_CORE
using System;

namespace Csla.Core
{
  /// <summary>
  /// Arguments object containing information about
  /// a property changing.
  /// </summary>
  public class PropertyChangingEventArgs : System.EventArgs
  {
    private string _propertyName = string.Empty;
    /// <summary>
    /// Creates an instnace of the object.
    /// </summary>
    /// <param name="propertyName">
    /// Name of the property that is changing.
    /// </param>
    public PropertyChangingEventArgs(string propertyName)
    {
      _propertyName = propertyName;
    }

    /// <summary>
    /// Gets the name of the changing property.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
    }
  }
}
#endif