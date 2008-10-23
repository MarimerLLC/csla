using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
    [Serializable()]
    public partial class HasRulesManager2 : BusinessBase<HasRulesManager2>
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
                    //ValidationRules.CheckRules("Name");
                    //MarkDirty();
                    PropertyHasChanged();
                }
            }
        }

        protected override void AddInstanceBusinessRules()
        {
            ValidationRules.AddInstanceRule(NameRequired, "Name");
            ValidationRules.AddInstanceRule(NameLength, new MaxLengthArgs("Name", 10));
            //ValidationRules.CheckRules();
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
            info.AddValue("_name", _name);
            base.OnGetState(info, mode);
          }

          protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
          {
            _name = info.GetValue<string>("_name");
            base.OnSetState(info, mode);
          }
        }

        public static void NewHasRulesManager2(EventHandler<DataPortalResult<HasRulesManager2>> completed)
        {
          Csla.DataPortal.BeginCreate<HasRulesManager2>(
            new Criteria(),
            completed);
        }

        public static void GetHasRulesManager2(string name, EventHandler<DataPortalResult<HasRulesManager2>> completed)
        {
          Csla.DataPortal.BeginFetch<HasRulesManager2>(
            new Criteria(name),
            completed);
        }

        public static void DeleteHasRulesManager2(string name)
        {
          Csla.DataPortal.BeginDelete<HasRulesManager2>(
            new Criteria(name),
            (o, e) => { });
        }
    }
}
