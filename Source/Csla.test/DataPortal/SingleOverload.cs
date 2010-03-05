using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortalTest
{
    [Serializable()]
    class SingleOverload : BusinessBase<SingleOverload>
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

        public static SingleOverload NewObject()
        {
          return Csla.DataPortal.Create<SingleOverload>();
        }
        public static SingleOverload NewObjectWithCriteria()
        {
          return Csla.DataPortal.Create<SingleOverload>(new OtherCriteria(0));
        }

        public static SingleOverload GetObject(int id)
        {
          return Csla.DataPortal.Fetch<SingleOverload>(new Criteria(id));
        }

        public static void DeleteObject(int id)
        {
          Csla.DataPortal.Delete<SingleOverload>(new Criteria(id));
        }

        private SingleOverload()
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

        [Serializable()]
        private class OtherCriteria
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }
            public OtherCriteria(int id)
            { _id = id; }
        }

        protected override void DataPortal_Create()
        {
          _id = 0;
          Csla.ApplicationContext.GlobalContext.Clear();
          ApplicationContext.GlobalContext.Add("SingleOverload", "Created0");
        }

        private void DataPortal_Create(Criteria criteria)
        {
            _id = 0;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("SingleOverload", "Created");
        }

        private void DataPortal_Create(OtherCriteria criteria)
        {
            _id = 0;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("SingleOverload", "Created1");
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            _id = criteria.Id;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("SingleOverload", "Fetched");
        }
        private void DataPortal_Fetch(OtherCriteria criteria)
        {
            _id = criteria.Id;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("SingleOverload", "Fetched1");
        }
        private void DataPortal_Delete(Criteria criteria)
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("SingleOverload", "Deleted");
        }
        private void DataPortal_Delete(OtherCriteria criteria)
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("SingleOverload", "Deleted1");
        }
        #endregion
    }
}
