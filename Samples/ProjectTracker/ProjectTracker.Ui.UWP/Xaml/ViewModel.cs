#if !XAMARIN && !NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create ViewModel objects,</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
  /// Base class used to create ViewModel objects,
  /// with pre-existing verbs for use by
  /// InvokeMethod or Invoke.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  public abstract class ViewModel<T> : ViewModelBase<T>
  {
    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    [Obsolete("Use SaveAsync", true)]
    public virtual void Save(object sender, ExecuteEventArgs e)
    {
      SaveAsync().RunSynchronously();
    }

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public virtual async void SaveAsync(object sender, ExecuteEventArgs e)
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