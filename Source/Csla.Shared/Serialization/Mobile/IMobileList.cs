//-----------------------------------------------------------------------
// <copyright file="IMobileList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extension of IMobileObject for list types</summary>
//-----------------------------------------------------------------------
namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Extension of IMobileObject for list types
  /// </summary>
  public interface IMobileList : IMobileObject
  {
    /// <summary>
    /// Sets the LoadListMode for the collection
    /// </summary>
    /// <param name="enabled">Enable or disable mode</param>
    void SetLoadListMode(bool enabled);
  }
}
