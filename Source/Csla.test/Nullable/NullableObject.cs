//-----------------------------------------------------------------------
// <copyright file="NullableObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
        public Nullable<int> _nullableIntMember;

        protected override object GetIdValue()
        {
            return _name;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
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
          return Csla.DataPortal.Create<NullableObject>(new Criteria()) as NullableObject;
        }

        public static NullableObject GetNullableObject(string name)
        {
          return Csla.DataPortal.Fetch<NullableObject>(new Criteria(name)) as NullableObject;
        }

        public static void DeleteNullableObject(string name)
        {
          Csla.DataPortal.Delete<NullableObject>(new Criteria(name));
        }

        public NullableObject()
        {
            AddBusinessRules();
        }

        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _name = crit._name;
            //Name = crit._name;
            Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Created");
        }

        protected void DataPortal_Fetch(object criteria)
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

        protected void DataPortal_Delete(object criteria)
        {
            //we would delete here
            Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Deleted");
        }


    }
}