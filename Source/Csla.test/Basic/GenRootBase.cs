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
        internal class Criteria : CriteriaBase<Criteria>
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

        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            TestResults.Add("GenRoot", "Created");
        }

        protected void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            MarkOld();
            TestResults.Add("GenRoot", "Fetched");
        }

        [Update]
		protected void DataPortal_Update()
        {
            if (IsDeleted)
            {
                //we would delete here
                TestResults.Add("GenRoot", "Deleted");
                MarkNew();
            }
            else
            {
                if (IsNew)
                {
                    //we would insert here
                    TestResults.Add("GenRoot", "Inserted");
                }
                else 
                {
                    //we would update here
                    TestResults.Add("GenRoot", "Updated");
                }
                MarkOld();
            }
        }

        [Delete]
		protected void DataPortal_Delete(object Criteria)
        {
            //we would delete here
            TestResults.Add("GenRoot", "Deleted");
        }
    }
}