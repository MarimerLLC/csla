using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
    [Serializable()]
    public class Root : BusinessBase<Root>
    {
        private string _data;

        private Children _children = null; //Csla.Test.Children.NewChildren();

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
                }
                else
                {
                    throw new System.Security.SecurityException("Property set not allowed");
                }
            }
        }

        public Children Children
        {
            get { return _children; }
        }

        /*public override bool IsDirty
        {
            get { return base.IsDirty || _children.IsDirty(); }
        }*/

        #region Criteria
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
        #endregion

        public static Root NewRoot()
        {
            return Csla.DataPortal.Create<Root>(new Criteria());
        }

        public static Root GetRoot(string data)
        {
            return Csla.DataPortal.Fetch<Root>(new Criteria(data));
        }

        public static void DeleteRoot(string data)
        {
            Csla.DataPortal.Delete(new Criteria(data));
        }

        private Root()
        {
            //prevent direct creation
        }

        protected override void DataPortal_Create(object criteria)
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
            Csla.ApplicationContext.GlobalContext.Add("clientcontext",
                ApplicationContext.ClientContext["clientcontext"]);

            Csla.ApplicationContext.GlobalContext.Add("globalcontext",
                ApplicationContext.GlobalContext["globalcontext"]);

            ApplicationContext.GlobalContext.Remove("globalcontext");
            ApplicationContext.GlobalContext["globalcontext"] = "new global value";

            Csla.ApplicationContext.GlobalContext.Add("Root", "Inserted");
        }

        protected override void DataPortal_Update()
        {
            //we would update here
            Csla.ApplicationContext.GlobalContext.Add("Root", "Updated");
        }

        protected override void DataPortal_DeleteSelf()
        {
            //we would delete here
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
            Csla.ApplicationContext.GlobalContext.Add("Deserialized", "root deserialized");
        }

        protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["dpinvoke"] = ApplicationContext.GlobalContext["global"];
        }

        protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["dpinvokecomplete"] = ApplicationContext.GlobalContext["global"];
        }    
    }
}
