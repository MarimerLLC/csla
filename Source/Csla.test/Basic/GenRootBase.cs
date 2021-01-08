//-----------------------------------------------------------------------
// <copyright file="GenRootBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
    [Serializable()]
    public abstract class GenRootBase : BusinessBase<GenRoot>
    {
        private string _data = "";

        protected override object GetIdValue()
        {
            return _data;
        }

        public string Data
        {
            get { return _data; }
            set {
                if (_data != value) 
                {
                    _data = value;
                    MarkDirty();
                }
            }
        }

        [Serializable()]
        private class Criteria : CriteriaBase<Criteria>
        {
            public string _data;

            public Criteria() 
            {
                _data = "<new>";
            }

            public Criteria(string data)
            {
                this._data = data;
            }
        }

        public static GenRoot NewRoot()
        {
            return (GenRoot)(Csla.DataPortal.Create<GenRoot>(new Criteria()));
        }

        public static GenRoot GetRoot(string data)
        {
          return (GenRoot)(Csla.DataPortal.Fetch<GenRoot>(new Criteria(data)));
        }

        public static void DeleteRoot(string data)
        {
          Csla.DataPortal.Delete<GenRoot>(new Criteria(data));
        }

        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Created");
        }

        protected void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            MarkOld();
            Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Fetched");
        }

        protected override void DataPortal_Update()
        {
            if (IsDeleted)
            {
                //we would delete here
                Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Deleted");
                MarkNew();
            }
            else
            {
                if (IsNew)
                {
                    //we would insert here
                    Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Inserted");
                }
                else 
                {
                    //we would update here
                    Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Updated");
                }
                MarkOld();
            }
        }

        protected void DataPortal_Delete(object Criteria)
        {
            //we would delete here
            Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Deleted");
        }
    }
}