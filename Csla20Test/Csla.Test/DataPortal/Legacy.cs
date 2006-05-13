using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortalTest
{
    [Serializable()]
    class Legacy : BusinessBase<Legacy>
    {
        #region Business Methods

        // TODO: add your own fields, properties and methods
        private int _id;

        public int id
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                CanWriteProperty(true);
                if (_id != value)
                {
                    _id = value;
                    PropertyHasChanged();
                }
            }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion


        #region Factory Methods

        public static Legacy NewObject()
        {
            return Csla.DataPortal.Create<Legacy>();
        }

        public static Legacy GetObject(int id)
        {
          return Csla.DataPortal.Fetch<Legacy>(new Criteria(id));
        }

        public static void DeleteObject(int id)
        {
          Csla.DataPortal.Delete(new Criteria(id));
        }

        private Legacy()
        { /* Require use of factory methods */ }

        #endregion

        #region Data Access

        [Serializable()]
        private class Criteria
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }
            public Criteria(int id)
            { _id = id; }
        }

        protected override void DataPortal_Create(object criteria)
        {
            _id = 0;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Legacy", "Created");
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            _id = ((Criteria)criteria).Id;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Legacy", "Fetched");
        }
        protected override void DataPortal_Insert()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Legacy", "Inserted");
        }

        protected override void DataPortal_Update()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Legacy", "Updated");
        }

        protected override void DataPortal_Delete(object criteria)
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Legacy", "Deleted");
        }
        protected override void DataPortal_DeleteSelf()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Legacy", "SelfDeleted");
        }

        #endregion
    }
}
