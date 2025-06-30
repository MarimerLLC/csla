﻿#if !XAMARIN && !NETFX_CORE && !MAUI
//-----------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create ViewModel objects,</summary>
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
  /// Base class used to create ViewModel objects,
  /// with pre-existing verbs for use by
  /// InvokeMethod or Invoke.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  public class ViewModel<T> : ViewModelBase<T>
  {
    /// <summary>
    /// Saves the Model, first committing changes if ManagedObjectLifetime is true.
    /// </summary>
    /// <param name="sender">The source of the event, typically the UI element that triggered the save operation.</param>
    /// <param name="e">The <see cref="ExecuteEventArgs"/> containing event data and parameters for the save operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
    public virtual async Task SaveAsync(object sender, ExecuteEventArgs e)
    {
      await SaveAsync();
    }

    /// <summary>
    /// Cancels changes made to the model 
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public virtual void Cancel(object sender, ExecuteEventArgs e)
    {
      DoCancel();
    }

    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    public virtual void AddNew(object sender, ExecuteEventArgs e)
    {
#if ANDROID || IOS
      BeginAddNew();
#else
      DoAddNew();
#endif
    }

    /// <summary>
    /// Removes an item from the Model (if it
    /// is a collection).
    /// </summary>
    public virtual void Remove(object sender, ExecuteEventArgs e)
    {
      DoRemove(e.MethodParameter);
    }

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    public virtual void Delete(object sender, ExecuteEventArgs e)
    {
      DoDelete();
    }
  }
}
#endif