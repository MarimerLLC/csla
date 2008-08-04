using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.ValidationRules
{
    [Serializable()]
    public class HasRulesManager : BusinessBase<HasRulesManager>
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
                    var x = ValidationRules.CheckRules("Name");
                    MarkDirty();
                }
            }
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
        private class Criteria
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
        }

        public static HasRulesManager NewHasRulesManager()
        {
            return Csla.DataPortal.Create(new Criteria()) as HasRulesManager;
        }

        public static HasRulesManager GetHasRulesManager(string name)
        {
            return Csla.DataPortal.Fetch(new Criteria(name)) as HasRulesManager;
        }

        public static void DeleteHasRulesManager(string name)
        {
            Csla.DataPortal.Delete(new Criteria(name));
        }

        private HasRulesManager()
        {
            //prevent direct creation
            AddBusinessRules();
        }

        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _name = crit._name;
            //Name = crit._name;
            Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Created");
            this.ValidationRules.CheckRules();
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _name = crit._name;
            MarkOld();
            Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Fetched");
        }

        protected override void DataPortal_Update()
        {
            if (IsDeleted)
            {
                //we would delete here
                Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Deleted");
                MarkNew();
            }
            else
            {
                if (this.IsNew)
                {
                    //we would insert here
                    Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Inserted");
                }
                else
                {
                    //we would update here
                    Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Updated");
                }
                MarkOld();
            }
        }

        protected override void DataPortal_Delete(object criteria)
        {
            //we would delete here
            Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Deleted");
        }


    }
}
