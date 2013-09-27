//-----------------------------------------------------------------------
// <copyright file="ExecuteEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Arguments passed to a method invoked</summary>
//-----------------------------------------------------------------------
using System;
using System.Windows;

#if __ANDROID__
namespace Csla.Axml
#else
namespace Csla.Xaml
#endif
{
  /// <summary>
  /// Arguments passed to a method invoked
  /// by the Execute trigger action.
  /// </summary>
  public class ExecuteEventArgs : EventArgs
  {
    /// <summary>
    /// The control that raised the event that
    /// triggered invocation of this method.
    /// </summary>
#if !__ANDROID__
    public FrameworkElement TriggerSource { get; set; }
#endif
    /// <summary>
    /// The MethodParameter value provided by
    /// the designer.
    /// </summary>
    public object MethodParameter { get; set; }
    /// <summary>
    /// The EventArgs parameter from the event
    /// that triggered invocation of this method.
    /// </summary>
    public object TriggerParameter { get; set; }
  }
}