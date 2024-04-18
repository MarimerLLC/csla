//-----------------------------------------------------------------------
// <copyright file="IModelCreator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>ASP.NET MVC model creator.</summary>
//-----------------------------------------------------------------------

namespace Csla.Web.Mvc
{
  /// <summary>
  /// ASP.NET MVC model creator.
  /// </summary>
  public interface IModelCreator
  {
    /// <summary>
    /// Creates a model object of the specified
    /// type.
    /// </summary>
    /// <param name="modelType">Type of model object to create.</param>
    /// <returns></returns>
    object CreateModel(Type modelType);
  }
}