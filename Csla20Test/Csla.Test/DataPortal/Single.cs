using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortalTest
{
    [Serializable()]
    class Single : BusinessBase<Single>
    {
        #region Business Methods

        // TODO: add your own fields, properties and methods
        private int _id;

        public int id
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                CanReadProperty(true);
                return _id;
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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

        public static Single NewObject()
        {
          return Csla.DataPortal.Create<Single>();
        }

        public static Single GetObject(int id)
        {
          return Csla.DataPortal.Fetch<Single>(new Criteria(id));
        }

        public static void DeleteObject(int id)
        {
          Csla.DataPortal.Delete(new Criteria(id));
        }

        private Single()
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

        private void DataPortal_Create(object criteria)
        {
            _id = 0;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Single", "Created");
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            _id = criteria.Id;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Single", "Fetched");
        }
        protected override void DataPortal_Insert()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Single", "Inserted");
        }

        protected override void DataPortal_Update()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Single", "Updated");
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Single", "Deleted");
        }
        protected override void DataPortal_DeleteSelf()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Single", "SelfDeleted");
        }

        #endregion
    }
}
