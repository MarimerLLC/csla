using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortal
{
    [Serializable()]
    public class DpRoot : BusinessBase<DpRoot>
    {
        private string _data;

        protected override object GetIdValue()
        {
            return _data;
        }

        public string Data
        {
            get
            {
                if (CanReadProperty())
                {
                    return _data;
                }
                else
                {
                    throw new System.Security.SecurityException("Property get not allowed");
                }
            }
            set
            {
                if (CanWriteProperty())
                {
                    if (_data != value)
                    {
                        _data = value;
                        PropertyHasChanged();
                    }
                    else
                    {
                        throw new System.Security.SecurityException("Property set not allowed");
                    }
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

        public static DpRoot NewRoot()
        {
            Criteria crit = new Criteria();
            return (Csla.DataPortal.Create<DpRoot>(crit));
        }

        public static DpRoot GetRoot(string data)
        {
            return (Csla.DataPortal.Fetch<DpRoot>(new Criteria(data)));
        }

        private DpRoot()
        {
            //prevent direct creation
        }

        protected override void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            MarkOld();
        }

        protected override void DataPortal_Insert()
        {
            //we would insert here
        }

        protected override void DataPortal_Update()
        {
            //we would update here
        }

        protected override void DataPortal_DeleteSelf()
        {
            //we would delete here
        }

        protected override void DataPortal_Delete(object criteria)
        {
            //we would delete here
        }

        protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["serverinvoke"] = true;
        }

        protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["serverinvokecomplete"] = true;
        }
    }
}
