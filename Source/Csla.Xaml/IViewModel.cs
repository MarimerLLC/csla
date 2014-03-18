//-----------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines a CSLA .NET viewmodel</summary>
//-----------------------------------------------------------------------


#if __ANDROID__
namespace Csla.Axml
#elif __IOS__
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