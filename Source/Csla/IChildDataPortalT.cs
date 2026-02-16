//-----------------------------------------------------------------------
// <copyright file="IChildDataPortalT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining the members of the child data portal type</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Core;

namespace Csla
{
  /// <summary>
  /// Interface defining the members of the child data portal type.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IChildDataPortal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>
    where T : ICslaObject
  {
    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    Task<T> CreateChildAsync(params object?[]? criteria);
    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    Task<T> FetchChildAsync(params object?[]? criteria);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="child">Object to update.</param>
    /// <param name="parameters">Additional, optional parameters to pass.</param>
    /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
    Task UpdateChildAsync(T child, params object?[]? parameters);
    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    T CreateChild(params object?[]? criteria);
    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    T FetchChild(params object?[]? criteria);
    /// <summary>
    /// Called by the business object's Save() method to
    /// insert, update or delete an object in the database.
    /// </summary>
    /// <remarks>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </remarks>
    /// <param name="child">A reference to the business object to be updated.</param>
    /// <param name="parameters">Optional arguments passed to child update methods.</param>
    /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
    void UpdateChild(T child, params object?[]? parameters);
  }
}
