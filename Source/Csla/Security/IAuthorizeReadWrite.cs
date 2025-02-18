//-----------------------------------------------------------------------
// <copyright file="IAuthorizeReadWrite.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the authorization interface through which an</summary>
//-----------------------------------------------------------------------

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
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    bool CanWriteProperty(string propertyName);
    /// <summary>
    /// Returns true if the user is allowed to write the
    /// to the specified property.
    /// </summary>
    /// <returns>true if write is allowed.</returns>
    /// <param name="property">Property to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    bool CanWriteProperty(Core.IPropertyInfo property);
    /// <summary>
    /// Returns true if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <returns>true if read is allowed.</returns>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    bool CanReadProperty(string propertyName);
    /// <summary>
    /// Returns true if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <returns>true if read is allowed.</returns>
    /// <param name="property">Property to read.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    bool CanReadProperty(Core.IPropertyInfo property);
    /// <summary>
    /// Returns true if the user is allowed to execute 
    /// the specified method.
    /// </summary>
    /// <returns>true if execute is allowed.</returns>
    /// <param name="methodName">Name of the method to execute.</param>
    /// <exception cref="ArgumentNullException"><paramref name="methodName"/> is <see langword="null"/>.</exception>
    bool CanExecuteMethod(string methodName);
    /// <summary>
    /// Returns true if the user is allowed to execute 
    /// the specified method.
    /// </summary>
    /// <returns>true if execute is allowed.</returns>
    /// <param name="method">Method to execute.</param>
    /// <exception cref="ArgumentNullException"><paramref name="method"/> is <see langword="null"/>.</exception>
    bool CanExecuteMethod(Core.IMemberInfo method);
  }
}