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
        private class Criteria : CriteriaBase
        {
            public string _data;

            public Criteria() : base(typeof(GenRoot))
            {
                _data = "<new>";
            }

            public Criteria(string data) : base(typeof(GenRoot))
            {
                this._data = data;
            }
        }

        public static GenRoot NewRoot()
        {
            return (GenRoot)(Csla.DataPortal.Create(new Criteria()));
        }

        public static GenRoot GetRoot(string data)
        {
            return (GenRoot)(Csla.DataPortal.Fetch(new Criteria(data)));
        }

        public static void DeleteRoot(string data)
        {
            Csla.DataPortal.Delete(new Criteria(data));
        }

        protected GenRootBase()
        {
            //prevent direct creation
        }

        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Created");
        }

        protected override void DataPortal_Fetch(object criteria)
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

        protected override void DataPortal_Delete(object Criteria)
        {
            //we would delete here
            Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Deleted");
        }
    }
}
