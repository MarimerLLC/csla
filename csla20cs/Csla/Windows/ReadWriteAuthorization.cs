using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace Csla.Windows
{

  [DesignerCategory("")]
  [ProvideProperty("ApplyAuthorization", typeof(Control))]
  public class ReadWriteAuthorization : Component, IExtenderProvider
  {

    private Dictionary<Control, bool> _sources = 
      new Dictionary<Control, bool>();

    public ReadWriteAuthorization(IContainer container)
    { container.Add(this); }

    public bool CanExtend(object extendee)
    {
      if (IsPropertyImplemented(extendee, "ReadOnly") 
        || IsPropertyImplemented(extendee, "Enabled"))
        return true;
      else
        return false;
    }

    public bool GetApplyAuthorization(Control source)
    {
      if (_sources.ContainsKey(source))
        return _sources[source];
      else
        return false;
    }

    public void SetApplyAuthorization(Control source, bool value)
    {
      if (_sources.ContainsKey(source))
        _sources[source] = value;
      else
        _sources.Add(source, value);
    }

    public void ResetControlAuthorization()
    {
      foreach (KeyValuePair<Control, bool> item in _sources)
      {
        if (item.Value)
        {
          // apply authorization rules
          ApplyAuthorizationRules(item.Key);
        }
      }
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
          // get the object property name
          string propertyName =
            binding.BindingMemberInfo.BindingField;
          // get the BusinessObject if appropriate
          if (bs.DataSource is Csla.Core.BusinessBase)
          {
            Csla.Core.BusinessBase ds =
              (Csla.Core.BusinessBase)bs.DataSource;

            ApplyReadRules(
              control, binding, propertyName,
              ds.CanReadProperty(propertyName));
            ApplyWriteRules(
              control, binding, propertyName,
              ds.CanWriteProperty(propertyName));
          }
          else if (bs.DataSource is Csla.Core.IReadOnlyObject)
          {
            Csla.Core.IReadOnlyObject ds =
              (Csla.Core.IReadOnlyObject)bs.DataSource;

            ApplyReadRules(
              control, binding, propertyName,
              ds.CanReadProperty(propertyName));
          }
        }
      }
    }

    private void ApplyReadRules(
      Control ctl, Binding binding, 
      string propertyName, bool canRead)
    {
      // enable/disable reading of the value
      if (canRead)
      {
        bool couldRead = ctl.Enabled;
        ctl.Enabled = true;
        binding.Format -= 
          new ConvertEventHandler(ReturnEmpty);
        if (!couldRead) binding.ReadValue();
      }
      else
      {
        ctl.Enabled = false;
        binding.Format += 
          new ConvertEventHandler(ReturnEmpty);

        // clear the control property
        PropertyInfo propertyInfo = 
          ctl.GetType().GetProperty(binding.PropertyName,
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
    }

    private void ApplyWriteRules(
      Control ctl, Binding binding, 
      string propertyName, bool canWrite)
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
      object result;
      if (desiredType.Equals(typeof(string)))
        result = string.Empty;
      else if (desiredType.Equals(typeof(DateTime)))
        result = DateTime.Now;
      else if (desiredType.IsPrimitive)
        result = 0;
      else
        result = null;
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
  }
}
