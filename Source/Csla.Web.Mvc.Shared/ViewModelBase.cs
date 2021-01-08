#if !NETSTANDARD2_0 && !NETCORE3_1 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create ViewModel objects that contain the Model object and related elements.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Base class used to create ViewModel objects that
  /// contain the Model object and related elements.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  public abstract class ViewModelBase<T> : IViewModel where T : class
  {
    object IViewModel.ModelObject
    {
      get { return ModelObject; }
      set { ModelObject = (T)value; }
    }

    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T ModelObject { get; set; }

    /// <summary>
    /// Saves the current Model object if the object
    /// implements Csla.Core.ISavable.
    /// </summary>
    /// <param name="modelState">Controller's ModelState object.</param>
    /// <param name="forceUpdate">if set to <c>true</c> force update.</param>
    /// <returns>
    /// true if the save succeeds.
    /// </returns>
    public virtual bool Save(ModelStateDictionary modelState, bool forceUpdate)
    {
      try
      {
        var savable = ModelObject as Csla.Core.ISavable;
        if (savable == null)
          throw new InvalidOperationException("Save");

        ModelObject = (T)savable.Save(forceUpdate);
        return true;
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          modelState.AddModelError("", ex.BusinessException.Message);
        else
          modelState.AddModelError("", ex.Message);
        return false;
      }
      catch (Exception ex)
      {
        modelState.AddModelError("", ex.Message);
        return false;
      }
    }
  }
}
#endif