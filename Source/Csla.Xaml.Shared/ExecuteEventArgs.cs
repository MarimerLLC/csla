#if !NETFX_CORE && !XAMARIN && !MAUI
//-----------------------------------------------------------------------
// <copyright file="ExecuteEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Arguments passed to a method invoked</summary>
//-----------------------------------------------------------------------
using System.Windows;

#if ANDROID
namespace Csla.Axml
#elif IOS
namespace Csla.Iosui
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
#if !ANDROID && !IOS
    /// <summary>
    /// The control that raised the event that
    /// triggered invocation of this method.
    /// </summary>
    public FrameworkElement? TriggerSource { get; }
#endif
    /// <summary>
    /// The MethodParameter value provided by
    /// the designer.
    /// </summary>
    public object? MethodParameter { get; }
    /// <summary>
    /// The EventArgs parameter from the event
    /// that triggered invocation of this method.
    /// </summary>
    public object? TriggerParameter { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ExecuteEventArgs"/>.
    /// </summary>
    /// <param name="methodParameter">The method parameter.</param>
    public ExecuteEventArgs(object? methodParameter) : this(methodParameter, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ExecuteEventArgs"/>.
    /// </summary>
    /// <param name="methodParameter">The method parameter.</param>
    /// <param name="triggerParameter">The trigger parameter.</param>
    /// <param name="triggerSource">The trigger source (only available in non Android and IOS environment).</param>
    public ExecuteEventArgs(object? methodParameter, object? triggerParameter
#if !ANDROID && !IOS
    , FrameworkElement? triggerSource
#endif
    )
    {
      MethodParameter = methodParameter;
      TriggerParameter = triggerParameter;
#if !ANDROID && !IOS
      TriggerSource = triggerSource;
#endif
    }
  }
}
#endif