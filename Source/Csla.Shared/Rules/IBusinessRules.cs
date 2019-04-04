﻿//-----------------------------------------------------------------------
// <copyright file="IBusinessRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the interface for BusinessRules class</summary>
//-----------------------------------------------------------------------
 namespace Csla.Rules
{
  /// <summary>
  /// Public interfacefor IBusinessRules
  /// </summary>
  public interface IBusinessRules
  {
    /// <summary>
    /// Gets the target business object
    /// </summary>
    /// <value>
    /// The business object.
    /// </value>
    object Target { get; }
  }
}