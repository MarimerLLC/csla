//-----------------------------------------------------------------------
// <copyright file="IChildDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining the members of the child data portal type</summary>
//-----------------------------------------------------------------------
using System.Threading.Tasks;

namespace Csla
{
  /// <summary>
  /// Interface defining the members of the child data portal type.
  /// </summary>
  public interface IChildDataPortal
  {
    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    Task<object> CreateChildAsync(params object[] criteria);
    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    Task<object> FetchChildAsync(params object[] criteria);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    /// <param name="parameters">Additional, optional parameters to pass</param>
    Task UpdateChildAsync(object obj, params object[] parameters);
    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    object CreateChild(params object[] criteria);
    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    object FetchChild(params object[] criteria);
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
    /// <param name="obj">A reference to the business object to be updated.</param>
    /// <param name="parameters"></param>
    /// <returns>A reference to the updated business object.</returns>
    void UpdateChild(object obj, params object[] parameters);
  }
}
