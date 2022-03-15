//-----------------------------------------------------------------------
// <copyright file="IUseFieldManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Csla.Core
{
  /// <summary>
  /// Indicates this type contains a FieldDataManager.
  /// </summary>
  internal interface IUseFieldManager
  {
    /// <summary>
    /// Gets a reference to the FieldDataManager for 
    /// this object.
    /// </summary>
    FieldManager.FieldDataManager FieldManager { get; }
  }
}
