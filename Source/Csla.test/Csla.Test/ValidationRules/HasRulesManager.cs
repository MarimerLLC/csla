using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  [Serializable()]
  public partial class HasRulesManager : BusinessBase<HasRulesManager>
  {
    private string _name = string.Empty;

    protected override object GetIdValue()
    {
      return _name;
    }

    public string Name
    {
      get { return _name; }
      set
      {
        if (this._name != value)
        {
          this._name = value;
          ValidationRules.CheckRules("Name");
          MarkDirty();
        }
      }
    }
    
    public static void NewHasRulesManager(EventHandler<DataPortalResult<HasRulesManager>> completed)
    {
      Csla.DataPortal.BeginCreate<HasRulesManager>(
        new Criteria(),
        completed);
    }

    public static void GetHasRulesManager(string name, EventHandler<DataPortalResult<HasRulesManager>> completed)
    {
      Csla.DataPortal.BeginFetch<HasRulesManager>(
        new Criteria(name),
        completed);
    }

    public static void DeleteHasRulesManager(string name)
    {
      Csla.DataPortal.BeginDelete<HasRulesManager>(
        new Criteria(name),
        (o, e) => { });
    }

    protected override void AddInstanceBusinessRules()
    {
      ValidationRules.AddInstanceRule(NameRequired, "Name");
      ValidationRules.AddInstanceRule(NameLength, new MaxLengthArgs("Name", 10));
      ValidationRules.CheckRules();
    }

    private bool NameRequired(object target, Validation.RuleArgs e)
    {
      if (this._name.Length > 0) return true;
      else
      {
        e.Description = "Name required";
        return false;
      }
    }

    private bool NameLength(object target, Validation.RuleArgs e)
    {
      if (this._name.Length <= ((MaxLengthArgs)e).MaxLength)
      {
        return true;
      }
      else
      {
        e.Description = "The value for " + e.PropertyName + " is too long";
        return false;
      }
    }

    [Serializable()]
    private class Criteria : CriteriaBase
    {
      public string _name;

      public Criteria()
      {
        _name = "<new>";
      }

      public Criteria(string name)
      {
        this._name = name;
      }

      protected override void OnGetState(SerializationInfo info, StateMode mode)
      {
        base.OnGetState(info, mode);
        info.AddValue("_name", _name);
      }

      protected override void OnSetState(SerializationInfo info, StateMode mode)
      {
        base.OnSetState(info, mode);
        _name = info.GetValue<string>("_name");
      }
    }
  }
}
