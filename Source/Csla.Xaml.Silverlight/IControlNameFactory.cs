﻿//-----------------------------------------------------------------------
// <copyright file="IControlNameFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines a factory object that maps</summary>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;

namespace Csla.Xaml
{
  /// <summary>
  /// Defines a factory object that maps
  /// controls to names and names to controls.
  /// </summary>
  public interface IControlNameFactory
  {
    /// <summary>
    /// Convert full name of the control to name that will be used for 
    /// creation of bookmakrs
    /// </summary>
    /// <param name="control">
    /// User Control or Control object that can be used
    /// by navigator as a target.
    /// </param>
    /// <returns>Short name of control used for bookmarks</returns>
    string ControlToControlName(Control control);

    /// <summary>
    /// Convert short name of control used for bookmarks to
    /// User Control or Control object
    /// </summary>
    /// <param name="controlName">Short name of control used for bookmarks</param>
    /// <returns>User Control or Control object</returns>
    Control ControlNameToControl(string controlName);
  }
}