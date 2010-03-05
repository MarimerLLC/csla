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
          Csla.DataPortal.Delete<T>(new Criteria(id));
        }

        #endregion

        #region Data Access

        [Serializable()]
        private class Criteria : CriteriaBase<Criteria>
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

        protected override void DataPortal_Create()
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
