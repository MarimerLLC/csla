//-----------------------------------------------------------------------
// <copyright file="ReadWriteAuthorization.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Windows Forms extender control that automatically</summary>
//-----------------------------------------------------------------------
using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace Csla.Windows
{
  /// <summary>
  /// Windows Forms extender control that automatically
  /// enables and disables detail form controls based
  /// on the authorization settings from a CSLA .NET 
  /// business object.
  /// </summary>
  [DesignerCategory("")]
  [ProvideProperty("ApplyAuthorization", typeof(Control))]
  [ToolboxItem(true), ToolboxBitmap(typeof(ReadWriteAuthorization), "Csla.Windows.ReadWriteAuthorization")]
  public class ReadWriteAuthorization : Component, IExtenderProvider
  {
    // this class keeps track of the control status 
    private class ControlStatus
    {
      public bool ApplyAuthorization { get; set; }
      public bool CanRead { get; set; }
    }

    private readonly Dictionary<Control, ControlStatus> _sources =
      new Dictionary<Control, ControlStatus>();

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="container">The container of the control.</param>
    public ReadWriteAuthorization(IContainer container)
    { container.Add(this); }

    /// <summary>
    /// Gets a value indicating whether the extender control
    /// can extend the specified control.
    /// </summary>
    /// <param name="extendee">The control to be extended.</param>
    /// <remarks>
    /// Any control implementing either a ReadOnly property or
    /// Enabled property can be extended.
    /// </remarks>
    public bool CanExtend(object extendee)
    {
      if (IsPropertyImplemented(extendee, "ReadOnly")
        || IsPropertyImplemented(extendee, "Enabled"))
        return true;
      else
        return false;
    }

    /// <summary>
    /// Gets the custom ApplyAuthorization extender
    /// property added to extended controls.
    /// </summary>
    /// <param name="source">Control being extended.</param>
    [Category("Csla")]
    public bool GetApplyAuthorization(Control source)
    {
      ControlStatus result;
      if (_sources.TryGetValue(source, out result))
        return result.ApplyAuthorization;
      else
        return false;
    }

    /// <summary>
    /// Sets the custom ApplyAuthorization extender
    /// property added to extended controls.
    /// </summary>
    /// <param name="source">Control being extended.</param>
    /// <param name="value">New value of property.</param>
    [Category("Csla")]
    public void SetApplyAuthorization(Control source, bool value)
    {
      ControlStatus status;
      if (_sources.TryGetValue(source, out status))
        status.ApplyAuthorization = value;
      else
        _sources.Add(
          source, 
          new ControlStatus { ApplyAuthorization = value, CanRead = true });
    }

    /// <summary>
    /// Causes the ReadWriteAuthorization control
    /// to apply authorization rules from the business
    /// object to all extended controls on the form.
    /// </summary>
    /// <remarks>
    /// Call this method to refresh the display of detail
    /// controls on the form any time the authorization
    /// rules may have changed. Examples include: after
    /// a user logs in or out, and after an object has
    /// been updated, inserted, deleted or retrieved
    /// from the database.
    /// </remarks>
    public void ResetControlAuthorization()
    {
      foreach (var item in _sources)
        if (item.Value.ApplyAuthorization)
          ApplyAuthorizationRules(item.Key);
    }

    private void ApplyAuthorizationRules(Control control)
    {
      foreach (Binding binding in control.DataBindings)
      {
        // get the BindingSource if appropriate
        if (binding.DataSource is BindingSource)
        {
          BindingSource bs =
            (BindingSource)binding.DataSource;
          // get the BusinessObject if appropriate
          Csla.Security.IAuthorizeReadWrite ds =
            bs.Current as Csla.Security.IAuthorizeReadWrite;
          if (ds != null)
          {
            // get the object property name
            string propertyName =
              binding.BindingMemberInfo.BindingField;

            ApplyReadRules(
              control, binding,
              ds.CanReadProperty(propertyName));
            ApplyWriteRules(
              control, binding,
              ds.CanWriteProperty(propertyName));
          }
        }
      }
    }

    private void ApplyReadRules(
      Control ctl, Binding binding,
      bool canRead)
    {
      var status = GetControlStatus(ctl);

      // enable/disable reading of the value
      if (canRead)
      {
        ctl.Enabled = true;
        // if !CanRead remove format event and refresh value 
        if (!status.CanRead)
        {
          binding.Format -= ReturnEmpty;
          binding.ReadValue();
        }
      }
      else
      {
        ctl.Enabled = false;
        if (status.CanRead)
        {
          binding.Format += ReturnEmpty;
        }

        // clear the value displayed by the control
        var propertyInfo = ctl.GetType().GetProperty(binding.PropertyName,
          BindingFlags.FlattenHierarchy |
          BindingFlags.Instance |
          BindingFlags.Public);
        if (propertyInfo != null)
        {
          propertyInfo.SetValue(ctl,
            GetEmptyValue(
            Utilities.GetPropertyType(
              propertyInfo.PropertyType)),
            new object[] { });
        }
      }

      // store new status
      status.CanRead = canRead;
    }

    private void ApplyWriteRules(
      Control ctl, Binding binding,
      bool canWrite)
    {
      if (ctl is Label) return;

      // enable/disable writing of the value
      PropertyInfo propertyInfo =
        ctl.GetType().GetProperty("ReadOnly",
        BindingFlags.FlattenHierarchy |
        BindingFlags.Instance |
        BindingFlags.Public);
      if (propertyInfo != null)
      {
        bool couldWrite =
          (!(bool)propertyInfo.GetValue(
          ctl, new object[] { }));
        propertyInfo.SetValue(
          ctl, !canWrite, new object[] { });
        if ((!couldWrite) && (canWrite))
          binding.ReadValue();
      }
      else
      {
        bool couldWrite = ctl.Enabled;
        ctl.Enabled = canWrite;
        if ((!couldWrite) && (canWrite))
          binding.ReadValue();
      }
    }

    private void ReturnEmpty(
      object sender, ConvertEventArgs e)
    {
      e.Value = GetEmptyValue(e.DesiredType);
    }

    private object GetEmptyValue(Type desiredType)
    {
      object result = null;
      if (desiredType.IsValueType)
        result = Activator.CreateInstance(desiredType);
      return result;
    }

    private static bool IsPropertyImplemented(
      object obj, string propertyName)
    {
      if (obj.GetType().GetProperty(propertyName,
        BindingFlags.FlattenHierarchy |
        BindingFlags.Instance |
        BindingFlags.Public) != null)
        return true;
      else
        return false;
    }

    private ControlStatus GetControlStatus(Control control)
    {
      return _sources[control];
    }
  }
}