//-----------------------------------------------------------------------
// <copyright file="AuthorizationActions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Authorization actions.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Authorization actions.
  /// </summary>
  public enum AuthorizationActions
  {
    /// <summary>
    /// Request to write or set a property value.
    /// </summary>
    WriteProperty,
    /// <summary>
    /// Request to read a property value.
    /// </summary>
    ReadProperty,
    /// <summary>
    /// Request to execute a method.
    /// </summary>
    ExecuteMethod,
    /// <summary>
    /// Request to create an object.
    /// </summary>
    CreateObject,
    /// <summary>
    /// Request to get or fetch an object.
    /// </summary>
    GetObject,
    /// <summary>
    /// Request to save or edit an object.
    /// </summary>
    EditObject,
    /// <summary>
    /// Request to delete an object.
    /// </summary>
    DeleteObject
  }
}