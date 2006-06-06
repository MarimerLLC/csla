using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortalTest
{
    [Serializable()]
    public abstract class SplitBase<T> : Csla.BusinessBase<T>
        where T : SplitBase<T>
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

        public static T NewObject()
        {
          return Csla.DataPortal.Create<T>();
        }

        public static T GetObject(int id)
        {
          return Csla.DataPortal.Fetch<T>(new Criteria(id));
        }

        public static void DeleteObject(int id)
        {
          Csla.DataPortal.Delete(new Criteria(id));
        }

        #endregion

        #region Data Access

        [Serializable()]
        private class Criteria : CriteriaBase
        {
            private int _id;
            public int Id
            {
                get { return _id; }
            }
            public Criteria(int id)
                : base(typeof(T))
            { _id = id; }
        }

        private void DataPortal_Create(object criteria)
        {
            _id = 0;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Split", "Created");
        }

        private void DataPortal_Fetch(Criteria criteria)
        {
            _id = criteria.Id;
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Split", "Fetched");
        }
        protected override void DataPortal_Insert()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Split", "Inserted");
        }

        protected override void DataPortal_Update()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Split", "Updated");
        }

        private void DataPortal_Delete(Criteria criteria)
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Split", "Deleted");
        }
        protected override void DataPortal_DeleteSelf()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ApplicationContext.GlobalContext.Add("Split", "SelfDeleted");
        }

        #endregion

    }
}
