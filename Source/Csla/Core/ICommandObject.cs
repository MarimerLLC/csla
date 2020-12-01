//-----------------------------------------------------------------------
// <copyright file="ICommandObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This interface is implemented by all</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// This interface is implemented by all
  /// Command objects.
  /// </summary>
  public interface ICommandObject : IBusinessObject, Csla.Serialization.Mobile.IMobileObject
  {
  }
}