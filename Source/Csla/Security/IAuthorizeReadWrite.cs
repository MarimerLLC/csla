//-----------------------------------------------------------------------
// <copyright file="IAuthorizeReadWrite.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the authorization interface through which an</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Security
{
  /// <summary>
  /// Defines the authorization interface through which an
  /// object can indicate which properties the current
  /// user can read and write.
  /// </summary>
  public interface IAuthorizeReadWrite
  {
    /// <summary>
    /// Returns true if the user is allowed to write the
    /// to the specified property.
    /// </summary>
    /// <returns>true if write is allowed.</returns>
    /// <param name="propertyName">Name of the property to write.</param>
    bool CanWriteProperty(string propertyName);
    /// <summary>
    /// Returns true if the user is allowed to write the
    /// to the specified property.
    /// </summary>
    /// <returns>true if write is allowed.</returns>
    /// <param name="property">Property to write.</param>
    bool CanWriteProperty(Csla.Core.IPropertyInfo property);
    /// <summary>
    /// Returns true if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <returns>true if read is allowed.</returns>
    /// <param name="propertyName">Name of the property to read.</param>
    bool CanReadProperty(string propertyName);
    /// <summary>
    /// Returns true if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <returns>true if read is allowed.</returns>
    /// <param name="property">Property to read.</param>
    bool CanReadProperty(Csla.Core.IPropertyInfo property);
    /// <summary>
    /// Returns true if the user is allowed to execute 
    /// the specified method.
    /// </summary>
    /// <returns>true if execute is allowed.</returns>
    /// <param name="methodName">Name of the method to execute.</param>
    bool CanExecuteMethod(string methodName);
    /// <summary>
    /// Returns true if the user is allowed to execute 
    /// the specified method.
    /// </summary>
    /// <returns>true if execute is allowed.</returns>
    /// <param name="method">Method to execute.</param>
    bool CanExecuteMethod(Csla.Core.IMemberInfo method);
  }
}