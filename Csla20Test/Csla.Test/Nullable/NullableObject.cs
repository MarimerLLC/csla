using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Nullable
{
    [Serializable()]
    public class NullableObject : BusinessBase<NullableObject>
    {
        private string _name = string.Empty;
        private Nullable<int> _nullableInteger;

        protected override object GetIdValue()
        {
            return _name;
        }

        public Nullable<int> NullableInteger
        {
            get { return _nullableInteger; }
            set
            {
                if (this._nullableInteger != value)
                {
                    this._nullableInteger = value;
                    MarkDirty();
                }
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

        public static NullableObject NewNullableObject()
        {
            return Csla.DataPortal.Create(new Criteria()) as NullableObject;
        }

        public static NullableObject GetNullableObject(string name)
        {
            return Csla.DataPortal.Fetch(new Criteria(name)) as NullableObject;
        }

        public static void DeleteNullableObject(string name)
        {
            Csla.DataPortal.Delete(new Criteria(name));
        }

        private NullableObject()
        {
            //prevent direct creation
            AddBusinessRules();
        }

        protected override void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _name = crit._name;
            //Name = crit._name;
            Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Created");
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _name = crit._name;
            MarkOld();
            Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Fetched");
        }

        protected override void DataPortal_Update()
        {
            if (IsDeleted)
            {
                //we would delete here
                Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Deleted");
                MarkNew();
            }
            else
            {
                if (this.IsNew)
                {
                    //we would insert here
                    Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Inserted");
                }
                else
                {
                    //we would update here
                    Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Updated");
                }
                MarkOld();
            }
        }

        protected override void DataPortal_Delete(object criteria)
        {
            //we would delete here
            Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Deleted");
        }


    }
}

