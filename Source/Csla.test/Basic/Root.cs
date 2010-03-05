using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
    [Serializable()]
    public class Root : BusinessBase<Root>
    {
        private string _data = "";

        private Children _children = Csla.Test.Basic.Children.NewChildren();

        protected override object GetIdValue()
        {
            return _data;
        }

        public string Data
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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

        private int _createdDomain;

        public int CreatedDomain
        {
          [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
          get 
          {
            CanReadProperty(true);
            return _createdDomain; 
          }
        }
	
        public Children Children
        {
            get { return _children; }
        }

        ///start editing
        ///
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || _children.IsDirty;
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
            Csla.DataPortal.Delete<Root>(new Criteria(data));
        }

        private Root()
        {
            //prevent direct creation
        }

        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            _createdDomain = AppDomain.CurrentDomain.Id;
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
            Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted self");
        }

        protected override void DataPortal_Delete(object criteria)
        {
            Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted");
        }

        protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Csla.ApplicationContext.GlobalContext.Add("Deserialized", "root Deserialized");
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