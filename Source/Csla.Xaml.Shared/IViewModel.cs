//-----------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a CSLA .NET viewmodel</summary>
//-----------------------------------------------------------------------
#if ANDROID
namespace Csla.Axml
#elif IOS
namespace Csla.Iosui
#else
namespace Csla.Xaml
#endif
{
  /// <summary>
  /// Defines a CSLA .NET viewmodel
  /// object.
  /// </summary>
  public interface IViewModel
  {
    /// <summary>
    /// Gets or sets the Model property
    /// of the viewmodel object.
    /// </summary>
    object Model { get; set; }
  }
}