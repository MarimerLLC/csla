//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Base class used to create ViewModel objects that contain the Model object and related elements.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
  }
}
