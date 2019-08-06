﻿//-----------------------------------------------------------------------
// <copyright file="BindingManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides the ability to bing properties on Axml controls to properties on CSLA objects.  Bindinds update normally when the control looses focus</summary>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif
using System;

namespace Csla.Iosui.Binding
{
  /// <summary>
  /// Provides the ability to bing properties on iOS UI controls to properties on CSLA objects.  Bindings update normally when the control looses focus.
  /// </summary>
  public class BindingManager
  {
    /// <summary>
    /// The UIView that is being used by the bindings.
    /// </summary>
    private readonly UIView _view;

    /// <summary>
    /// Creates a new instance of the binding manager.
    /// </summary>
    /// <param name="view">The Android activity that is using the view with the controls to bind to.</param>
    public BindingManager(UIView view)
    {
      _view = view;
      Bindings = new List<Binding>();
    }

    /// <summary>
    /// A list of bindings that have been added to the manager.
    /// </summary>
    public List<Binding> Bindings { get; private set; }

    /// <summary>
    /// Adds a new binding to be managed.
    /// </summary>
    /// <param name="control">A reference to the control to bind to.</param>
    /// <param name="targetProperty">The name of the property on the view to bind to.</param>
    /// <param name="source">A reference to the object that will be bound to the control.</param>
    /// <param name="sourceProperty">The name of the property on the object that will be bound to the target property.</param>
    /// <param name="bindingDirection">Indicates if the binding is one way or two way.</param>
    public void Add(UIView control, string targetProperty, object source, string sourceProperty, BindingDirection bindingDirection)
    {
        Add(new Binding(control, targetProperty, source, sourceProperty, bindingDirection));
    }

    /// <summary>
    /// Adds a new binding to be managed.
    /// </summary>
    /// <param name="binding">The new binding to add.</param>
    public void Add(Binding binding)
    {
      Remove(binding.Target, binding.TargetProperty.Name, binding.Source, binding.SourceProperty.Name);
      Bindings.Add(binding);
    }

    /// <summary>
    /// Removes the binding matching the supplied parameters from the binding manager.  If no bindings matched the method does nothing.
    /// </summary>
    /// <param name="target">A reference to the bound control.</param>
    /// <param name="targetProperty">The name of the property on the view that is bound to.</param>
    /// <param name="source">A reference to the object that is bound to.</param>
    /// <param name="sourceProperty">The name of the property on the object that that is bound to the target property.</param>
    public void Remove(UIView target, string targetProperty, object source, string sourceProperty)
    {
      var binding = Bindings.FirstOrDefault(r => ReferenceEquals(r.Target, target) &&
                                                 r.TargetProperty.Name == targetProperty &&
                                                 ReferenceEquals(r.Source, source) &&
                                                 r.SourceProperty.Name == sourceProperty);
      if (binding != null)
        Remove(binding);
    }

    /// <summary>
    /// Removes the supplied binding from the binding manager.
    /// </summary>
    /// <param name="binding">The binding to remove.</param>
    public void Remove(Binding binding)
    {
      binding.Dispose();
      Bindings.Remove(binding);
    }

    /// <summary>
    /// Removes all bindings from the binding manager.
    /// </summary>
    public void RemoveAll()
    {
        for (var i = Bindings.Count - 1; i >= 0; i--)
        {
            Remove(Bindings[i]);
        }
    }

    /// <summary>
    /// Returns all bindings for the supplied view.
    /// </summary>
    /// <param name="view">A reference to the view to return bindings for.</param>
    /// <returns>A enumerable containing the bindings for the supplied view.</returns>
    public IEnumerable<Binding> GetBindingsForView(UIView view)
    {
      return Bindings.Where(r => ReferenceEquals(r.Target.ViewForBaselineLayout, view));
    }

    /// <summary>
    /// Updates bindings with the current values in the supplied control.
    /// </summary>
    /// <param name="control">The control to update bindings for.</param>
    public void UpdateSourceForView(UIView control)
    {
      foreach (var item in GetBindingsForView(control))
        item.UpdateSource();
    }

    /// <summary>
    /// Updates bindings on the view that is in current focus on the activity supplied to the BindingManager.
    /// </summary>
    public void UpdateSourceForLastView()
    {
      var view = _view.Subviews.FirstOrDefault(v => v.IsFirstResponder);
      if (view != null)
        UpdateSourceForView(view);
    }
  }
}