#if !NETFX_CORE && !XAMARIN
//-----------------------------------------------------------------------
// <copyright file="IErrorDialog.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining the interaction between</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Xaml
{
  /// <summary>
  /// Interface defining the interaction between
  /// a CslaDataSource and an error dialog control.
  /// </summary>
  public interface IErrorDialog
  {
    /// <summary>
    /// Method called by the CslaDataProvider when the
    /// error dialog should register any events it
    /// wishes to handle from the CslaDataProvider.
    /// </summary>
    /// <param name="source">Data provider control.</param>
    void Register(object source);
  }
}
#endif