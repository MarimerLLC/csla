//-----------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a CSLA .NET MVC viewmodel object.</summary>
//-----------------------------------------------------------------------
namespace Csla.Web.Mvc
{
  /// <summary>
  /// Defines a CSLA .NET MVC viewmodel object.
  /// </summary>
  public interface IViewModel
  {
    /// <summary>
    /// Object property for the contained business object
    /// </summary>
    object ModelObject { get; set; }
  }
}
