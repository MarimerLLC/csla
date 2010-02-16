using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Xaml
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
