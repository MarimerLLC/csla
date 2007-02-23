using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.RollBack
{
    [Serializable()]
    public class RollbackRoot : BusinessBase<RollbackRoot>
    {
        private string _data = "";
        private bool _fail = false;

        protected override object GetIdValue()
        {
            return _data;
        }

        public string Data
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    MarkDirty();
                }
            }
        }

        public bool Fail
        {
            get { return _fail; }
            set
            {
                if (_fail != value)
                {
                    _fail = value;
                    MarkDirty();
                }
            }
        }

        [Serializable()]
        private class Criteria
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

        public static RollbackRoot NewRoot()
        {
            return ((RollbackRoot)(Csla.DataPortal.Create(new Criteria())));
        }

        public static RollbackRoot GetRoot(string data)
        {
            return ((RollbackRoot)(Csla.DataPortal.Fetch(new Criteria(data))));
        }

        public static void DeleteRoot(string data)
        {
            Csla.DataPortal.Delete(new Criteria(data));
        }

        private RollbackRoot()
        {
            //prevent direct creation
        }

        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            Csla.ApplicationContext.GlobalContext.Add("Root", "Created");
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            MarkOld();
            Csla.ApplicationContext.GlobalContext.Add("Root", "Fetched");
        }

        protected override void DataPortal_Insert()
        {
            Csla.ApplicationContext.GlobalContext.Add("Root", "Inserted");
        }

        protected override void DataPortal_Update()
        {
            //we would update here
            Csla.ApplicationContext.GlobalContext.Add("Root", "Updated");
            
            if (_fail)
                throw new Exception("fail Update");
        }

        protected override void DataPortal_DeleteSelf()
        {
            Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted self");
        }

        protected override void DataPortal_Delete(object criteria)
        {
            //we would delete here
            Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted");
        }

        protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            base.OnDeserialized(context);
            Csla.ApplicationContext.GlobalContext.Add("Deserialized", "root Deserialized");
        }
        



    }
}
